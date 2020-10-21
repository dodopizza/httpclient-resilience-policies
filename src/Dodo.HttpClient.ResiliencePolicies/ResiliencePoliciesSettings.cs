using Dodo.HttpClientResiliencePolicies.CircuitBreakerSettings;
using Dodo.HttpClientResiliencePolicies.RetrySettings;
using Dodo.HttpClientResiliencePolicies.TimeoutPolicySettings;

namespace Dodo.HttpClientResiliencePolicies
{
	public class ResiliencePoliciesSettings
	{
		public ResiliencePoliciesSettings()
		{
			OverallTimeoutPolicySettings = new OverallTimeoutPolicySettings();
			TimeoutPerTryPolicySettings = new TimeoutPerTryPolicySettings();
			RetrySettings = new JitterRetrySettings();
			CircuitBreakerSettings = new HttpClientResiliencePolicies.CircuitBreakerSettings.CircuitBreakerSettings();
		}

		public ITimeoutPolicySettings OverallTimeoutPolicySettings { get; set; }

		public ITimeoutPolicySettings TimeoutPerTryPolicySettings { get; set; }

		public IRetrySettings RetrySettings { get; set; }

		public ICircuitBreakerSettings CircuitBreakerSettings { get; set; }
	}
}
