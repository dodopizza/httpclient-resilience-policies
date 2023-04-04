using System;
using System.Net;
using System.Net.Http;
using Dodo.HttpClientResiliencePolicies.CircuitBreakerPolicy;
using Dodo.HttpClientResiliencePolicies.RetryPolicy;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Registry;
using Polly.Timeout;

namespace Dodo.HttpClientResiliencePolicies
{
	/// <summary>
	/// Extension methods for configuring <see cref="IHttpClientBuilder"/> with Polly retry, timeout, circuit breaker policies.
	/// </summary>
	public static class PoliciesHttpClientBuilderExtensions
	{
		/// <summary>
		/// Adds pre-configured resilience policies.
		/// </summary>
		/// <param name="clientBuilder">Configured HttpClient builder.</param>
		/// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the client.</returns>
		public static IHttpClientBuilder AddResiliencePolicies(
			this IHttpClientBuilder clientBuilder)
		{
			return clientBuilder
				.AddResiliencePolicies(new ResiliencePoliciesSettings());
		}

		/// <summary>
		/// Adds and configures custom resilience policies.
		/// </summary>
		/// <param name="clientBuilder">Configured HttpClient builder.</param>
		/// <param name="settings">Custom resilience policy settings.</param>
		/// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the client.</returns>
		public static IHttpClientBuilder AddResiliencePolicies(
			this IHttpClientBuilder clientBuilder,
			ResiliencePoliciesSettings settings)
		{
			return clientBuilder
				.AddTimeoutPolicy(settings.OverallTimeout)
				.AddRetryPolicy(settings.RetryPolicySettings)
				.AddCircuitBreakerPolicy(settings.CircuitBreakerPolicySettings)
				.AddTimeoutPolicy(settings.TimeoutPerTry);
		}

		private static IHttpClientBuilder AddRetryPolicy(
			this IHttpClientBuilder clientBuilder,
			RetryPolicySettings settings)
		{
			return clientBuilder
				.AddPolicyHandler(HttpPolicyExtensions
					.HandleTransientHttpError()
					.Or<TimeoutRejectedException>()
					.WaitAndRetryAsync(settings));
		}

		private static IHttpClientBuilder AddCircuitBreakerPolicy(
			this IHttpClientBuilder clientBuilder,
			CircuitBreakerPolicySettings settings)
		{
			// This implementation takes into consideration situations
			// when you use the only HttpClient against different hosts.
			// In this case we want to have separate CircuitBreaker metrics for each host.
			// It allows us avoid situations when all requests to all hosts
			// will be stopped by CircuitBreaker due to single host is not available.
			var registry = new PolicyRegistry();
			return clientBuilder.AddPolicyHandler(message =>
			{
				var policyKey = message.RequestUri.Host;
				var policy = registry.GetOrAdd(policyKey, BuildCircuitBreakerPolicy(settings));
				return policy;
			});
		}

		private static IAsyncPolicy<HttpResponseMessage> BuildCircuitBreakerPolicy(
			CircuitBreakerPolicySettings settings)
		{
			return HttpPolicyExtensions
				.HandleTransientHttpError()
				.Or<TimeoutRejectedException>()
				.OrResult(settings.AdditionalFailureResultFilter)
				.AdvancedCircuitBreakerAsync(settings);
		}

		private static IHttpClientBuilder AddTimeoutPolicy(
			this IHttpClientBuilder httpClientBuilder,
			TimeSpan timeout)
		{
			return httpClientBuilder.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(timeout));
		}
	}
}
