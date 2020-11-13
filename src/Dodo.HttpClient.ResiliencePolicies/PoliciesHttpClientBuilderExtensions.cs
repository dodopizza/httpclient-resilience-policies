using Dodo.HttpClientResiliencePolicies.Core;
using Microsoft.Extensions.DependencyInjection;


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
			clientBuilder
				.UsePolly()
				.AddPolicy(settings.OverallTimeoutPolicySettings)
				.AddPolicy(settings.RetrySettings)
				.AddPolicy(settings.CircuitBreakerSettings)
				.AddPolicy(settings.TimeoutPerTryPolicySettings);

			return clientBuilder;
		}

		private static IPolicyBuilder UsePolly(this IHttpClientBuilder httpClientBuilder)
		{
			return new Polly.PolicyBuilder(httpClientBuilder);
		}
	}
}
