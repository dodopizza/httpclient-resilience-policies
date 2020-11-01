using Dodo.HttpClientResiliencePolicies.CircuitBreakerSettings;
using Dodo.HttpClientResiliencePolicies.RetrySettings;
using Dodo.HttpClientResiliencePolicies.TimeoutPolicySettings;

namespace Dodo.HttpClientResiliencePolicies
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
			RetrySettings = new RetryPolicySettings();
			CircuitBreakerSettings = new CircuitBreakerPolicySettings();
		}
	}
}
