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
			string clientName = null) where TClientInterface : class
			where TClientImplementation : class, TClientInterface
		{
			return AddJsonClient<TClientInterface, TClientImplementation>(sc, baseAddress, (s) => new ResiliencePoliciesSettings(), clientName);
		}

		/// <summary>
		/// Adds the <see cref="IHttpClientFactory"/> and related services to the <see cref="IServiceCollection"/>
		/// with pre-configured JSON headers, client Timeout and default policies.
		/// </summary>
		/// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the client.</returns>
		public static IHttpClientBuilder AddJsonClient<TClientInterface, TClientImplementation>(
			this IServiceCollection sc,
			Uri baseAddress,
			Action<ResiliencePoliciesSettings> settings,
			string clientName = null) where TClientInterface : class
			where TClientImplementation : class, TClientInterface
		{
			var options = new ResiliencePoliciesSettings();
			settings(options);

			var delta = TimeSpan.FromMilliseconds(1000);
			Action<HttpClient> defaultClient = (client) =>
			{
				client.BaseAddress = baseAddress;
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				client.Timeout = options.OverallTimeoutPolicySettings.Timeout + delta;
			};

			var httpClientBuilder = string.IsNullOrEmpty(clientName)
				? sc.AddHttpClient<TClientInterface, TClientImplementation>(defaultClient)
				: sc.AddHttpClient<TClientInterface, TClientImplementation>(clientName, defaultClient);

			httpClientBuilder
				.AddTimeoutPolicy(options.OverallTimeoutPolicySettings)
				.AddRetryPolicy(options.RetrySettings)
				.AddCircuitBreakerPolicy(options.CircuitBreakerSettings)
				.AddTimeoutPolicy(options.TimeoutPerTryPolicySettings);

			return httpClientBuilder;
		}

		private static IHttpClientBuilder AddRetryPolicy(
			this IHttpClientBuilder clientBuilder,
			RetrySettings.IRetryPolicySettings settings)
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
			ICircuitBreakerPolicySettings settings)
		{
			if (settings.IsHostSpecificOn)
			{
				var registry = new PolicyRegistry();
				return clientBuilder.AddPolicyHandler(message =>
				{
					var policyKey = message.RequestUri.Host;
					var policy = registry.GetOrAdd(policyKey, BuildCircuitBreakerPolicy(settings));
					return policy;
				});
			}
			else
			{
				return clientBuilder.AddPolicyHandler(BuildCircuitBreakerPolicy(settings));
			}
		}

		private static AsyncCircuitBreakerPolicy<HttpResponseMessage> BuildCircuitBreakerPolicy(
			ICircuitBreakerPolicySettings settings)
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
