using System.Net.Http;
using Dodo.HttpClientResiliencePolicies.CircuitBreakerPolicy;
using Polly;

namespace Dodo.HttpClientResiliencePolicies.RetryPolicy
{
	internal static class PolicyBuilderExtension
	{
		public static IAsyncPolicy<HttpResponseMessage> WaitAndRetryAsync(
			this PolicyBuilder<HttpResponseMessage> policyBuilder,
			IRetryPolicySettings settings)
		{
			var handler = new RetryPolicyHandler(settings);
			return policyBuilder
				.WaitAndRetryAsync(
					handler.RetryCount,
					handler.SleepDurationProvider,
					handler.OnRetry);
		}

		public static IAsyncPolicy<HttpResponseMessage> AdvancedCircuitBreakerAsync(
			this PolicyBuilder<HttpResponseMessage> policyBuilder,
			ICircuitBreakerPolicySettings settings)
		{
			return policyBuilder
				.AdvancedCircuitBreakerAsync(
					settings.FailureThreshold,
					settings.SamplingDuration,
					settings.MinimumThroughput,
					settings.DurationOfBreak,
					settings.OnBreak,
					settings.OnReset,
					settings.OnHalfOpen);
		}

	}
}
