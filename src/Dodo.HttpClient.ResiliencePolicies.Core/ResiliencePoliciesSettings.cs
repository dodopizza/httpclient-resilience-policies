using Dodo.HttpClientResiliencePolicies.Core.CircuitBreakerPolicy;
using Dodo.HttpClientResiliencePolicies.Core.RetryPolicy;
using Dodo.HttpClientResiliencePolicies.Core.TimeoutPolicy;

namespace Dodo.HttpClientResiliencePolicies.Core
{
	public class ResiliencePoliciesSettings
	{
		public ITimeoutPolicySettings OverallTimeoutPolicySettings { get; set; }

		public ITimeoutPolicySettings TimeoutPerTryPolicySettings { get; set; }

		public IRetryPolicySettings RetrySettings { get; set; }

		public ICircuitBreakerPolicySettings CircuitBreakerSettings { get; set; }

		public ResiliencePoliciesSettings()
		{
			OverallTimeoutPolicySettings = new OverallTimeoutPolicySettings();
			TimeoutPerTryPolicySettings = new TimeoutPerTryPolicySettings();
			RetrySettings = RetryPolicySettings.Default();
			CircuitBreakerSettings = new CircuitBreakerPolicySettings();
		}
	}
}
