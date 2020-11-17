using System;
using Dodo.HttpClientResiliencePolicies.CircuitBreakerPolicy;
using Dodo.HttpClientResiliencePolicies.RetryPolicy;
using Dodo.HttpClientResiliencePolicies.TimeoutPolicy;

namespace Dodo.HttpClientResiliencePolicies
{
	public class ResiliencePoliciesSettings
	{
		public ITimeoutPolicySettings OverallTimeoutPolicySettings { get; set; }

		public ITimeoutPolicySettings TimeoutPerTryPolicySettings { get; set; }

		public IRetryPolicySettings RetryPolicySettings { get; set; }

		public ICircuitBreakerPolicySettings CircuitBreakerPolicySettings { get; set; }

		public ResiliencePoliciesSettings()
		{
			OverallTimeoutPolicySettings = new OverallTimeoutPolicySettings();
			TimeoutPerTryPolicySettings = new TimeoutPerTryPolicySettings();
			RetryPolicySettings = new RetryPolicySettings();
			CircuitBreakerPolicySettings = new CircuitBreakerPolicySettings();
		}

		public ResiliencePoliciesSettings(
			TimeSpan overallTimeout,
			TimeSpan timeoutPerTry,
			IRetryPolicySettings retryPolicyPolicySettings,
			ICircuitBreakerPolicySettings circuitBreakerPolicyPolicySettings)
		{
			OverallTimeoutPolicySettings = new OverallTimeoutPolicySettings(overallTimeout);
			TimeoutPerTryPolicySettings = new TimeoutPerTryPolicySettings(timeoutPerTry);
			RetryPolicySettings = retryPolicyPolicySettings;
			CircuitBreakerPolicySettings = circuitBreakerPolicyPolicySettings;
		}
	}
}
