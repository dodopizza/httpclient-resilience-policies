using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Dodo.HttpClientResiliencePolicies.CircuitBreakerSettings;
using Dodo.HttpClientResiliencePolicies.RetrySettings;
using Dodo.HttpClientResiliencePolicies.TimeoutPolicySettings;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Registry;
using Polly.Timeout;

namespace Dodo.HttpClientResiliencePolicies
{
	/// <summary>
	/// Extension methods for configuring <see cref="IHttpClientBuilder"/> with Polly retry, timeout, circuit breaker policies.
	/// </summary>
	public static class HttpClientBuilderExtensions
	{
		/// <summary>
		/// Adds the <see cref="IHttpClientFactory"/> and related services to the <see cref="IServiceCollection"/>
		/// with pre-configured JSON headers, client Timeout and default policies.
		/// </summary>
		/// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the client.</returns>
		public static IHttpClientBuilder AddJsonClient<TClientInterface, TClientImplementation>(
			this IServiceCollection sc,
			Uri baseAddress,
			ResiliencePoliciesSettings settings,
			string clientName = null) where TClientInterface : class
			where TClientImplementation : class, TClientInterface
		{
			var delta = TimeSpan.FromMilliseconds(1000);
			Action<HttpClient> defaultClient = (client) =>
			{
				client.BaseAddress = baseAddress;
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				client.Timeout = settings.OverallTimeoutPolicySettings.Timeout + delta;
			};

			var httpClientBuilder = string.IsNullOrEmpty(clientName)
				? sc.AddHttpClient<TClientInterface, TClientImplementation>(defaultClient)
				: sc.AddHttpClient<TClientInterface, TClientImplementation>(clientName, defaultClient);

			httpClientBuilder.AddDefaultPolicies(settings);

			return httpClientBuilder;
		}

		/// <summary>
		/// Adds pre-configured default policies.
		/// </summary>
		/// <param name="clientBuilder">Configured HttpClient builder.</param>
		/// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the client.</returns>
		public static IHttpClientBuilder AddDefaultPolicies(
			this IHttpClientBuilder clientBuilder)
		{
			return clientBuilder
				.AddDefaultPolicies(new ResiliencePoliciesSettings());
		}

		/// <summary>
		/// Adds and configures custom policies.
		/// </summary>
		/// <param name="clientBuilder">Configured HttpClient builder.</param>
		/// <param name="settings">Custom policy settings.</param>
		/// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the client.</returns>
		public static IHttpClientBuilder AddDefaultPolicies(
			this IHttpClientBuilder clientBuilder,
			ResiliencePoliciesSettings settings)
		{
			return clientBuilder
				.AddTimeoutPolicy(settings.OverallTimeoutPolicySettings)
				.AddRetryPolicy(settings.RetrySettings)
				.AddCircuitBreakerPolicy(settings.CircuitBreakerSettings)
				.AddTimeoutPolicy(settings.TimeoutPerTryPolicySettings);
		}

		/// <summary>
		/// Adds pre-configured default policies to use single HttpClient against multiple hosts.
		/// </summary>
		/// <param name="clientBuilder">Configured HttpClient builder.</param>
		/// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the client.</returns>
		public static IHttpClientBuilder AddDefaultHostSpecificPolicies(
			this IHttpClientBuilder clientBuilder)
		{
			return clientBuilder
				.AddDefaultHostSpecificPolicies(new ResiliencePoliciesSettings());
		}

		/// <summary>
		/// Adds and configures custom policies to use single HttpClient against multiple hosts.
		/// </summary>
		/// <param name="clientBuilder">Configured HttpClient builder.</param>
		/// <param name="settings">Custom policy settings.</param>
		/// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the client.</returns>
		public static IHttpClientBuilder AddDefaultHostSpecificPolicies(
			this IHttpClientBuilder clientBuilder,
			ResiliencePoliciesSettings settings)
		{
			return clientBuilder
				.AddTimeoutPolicy(settings.OverallTimeoutPolicySettings)
				.AddRetryPolicy(settings.RetrySettings)
				.AddHostSpecificCircuitBreakerPolicy(settings.CircuitBreakerSettings)
				.AddTimeoutPolicy(settings.TimeoutPerTryPolicySettings);
		}

		private static IHttpClientBuilder AddRetryPolicy(
			this IHttpClientBuilder clientBuilder,
			IRetrySettings settings)
		{
			return clientBuilder
				.AddPolicyHandler(HttpPolicyExtensions
					.HandleTransientHttpError()
					.Or<TimeoutRejectedException>()
					.WaitAndRetryAsync(
						settings.RetryCount,
						settings.SleepDurationProvider,
						settings.OnRetry));
		}

		private static IHttpClientBuilder AddCircuitBreakerPolicy(
			this IHttpClientBuilder clientBuilder,
			ICircuitBreakerSettings settings)
		{
			return clientBuilder.AddPolicyHandler(BuildCircuitBreakerPolicy(settings));
		}

		private static IHttpClientBuilder AddHostSpecificCircuitBreakerPolicy(
			this IHttpClientBuilder clientBuilder,
			ICircuitBreakerSettings settings)
		{
			var registry = new PolicyRegistry();
			return clientBuilder.AddPolicyHandler(message =>
			{
				var policyKey = message.RequestUri.Host;
				var policy = registry.GetOrAdd(policyKey, BuildCircuitBreakerPolicy(settings));
				return policy;
			});
		}

		private static AsyncCircuitBreakerPolicy<HttpResponseMessage> BuildCircuitBreakerPolicy(
			ICircuitBreakerSettings settings)
		{
			return HttpPolicyExtensions
				.HandleTransientHttpError()
				.Or<TimeoutRejectedException>()
				.OrResult(r => r.StatusCode == (HttpStatusCode) 429) // Too Many Requests
				.AdvancedCircuitBreakerAsync(
					settings.FailureThreshold,
					settings.SamplingDuration,
					settings.MinimumThroughput,
					settings.DurationOfBreak,
					settings.OnBreak,
					settings.OnReset,
					settings.OnHalfOpen);
		}

		private static IHttpClientBuilder AddTimeoutPolicy(this IHttpClientBuilder httpClientBuilder, ITimeoutPolicySettings settings)
		{
			return httpClientBuilder.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(settings.Timeout));
		}
	}
}
