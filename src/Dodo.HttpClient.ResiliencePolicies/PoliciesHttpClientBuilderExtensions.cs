using System.Net;
using System.Net.Http;
using Dodo.HttpClientResiliencePolicies.CircuitBreakerPolicy;
using Dodo.HttpClientResiliencePolicies.RetryPolicy;
using Dodo.HttpClientResiliencePolicies.TimeoutPolicy;
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
				.AddTimeoutPolicy(settings.OverallTimeoutPolicySettings)
				.AddRetryPolicy(settings.RetrySettings)
				.AddCircuitBreakerPolicy(settings.CircuitBreakerSettings)
				.AddTimeoutPolicy(settings.TimeoutPerTryPolicySettings);
		}

		private static IHttpClientBuilder AddRetryPolicy(
			this IHttpClientBuilder clientBuilder,
			IRetryPolicySettings settings)
		{
			return clientBuilder
				.AddPolicyHandler(HttpPolicyExtensions
					.HandleTransientHttpError()
					.Or<TimeoutRejectedException>()
					.WaitAndRetryAsync(
						settings.RetryCount,
						settings.SleepDurationProvider,
						settings.OnRetryForPolly));
		}

		private static IHttpClientBuilder AddCircuitBreakerPolicy(
			this IHttpClientBuilder clientBuilder,
			ICircuitBreakerPolicySettings settings)
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

		private static IHttpClientBuilder AddTimeoutPolicy(
			this IHttpClientBuilder httpClientBuilder,
			ITimeoutPolicySettings settings)
		{
			return httpClientBuilder.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(settings.Timeout));
		}
	}
}
