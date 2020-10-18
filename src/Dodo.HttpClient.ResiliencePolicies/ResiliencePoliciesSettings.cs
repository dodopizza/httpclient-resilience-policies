using Dodo.HttpClientResiliencePolicies.CircuitBreakerSettings;
using Dodo.HttpClientResiliencePolicies.RetrySettings;
using Dodo.HttpClientResiliencePolicies.TimeoutPolicySettings;

namespace Dodo.HttpClientResiliencePolicies
{
	public class ResiliencePoliciesSettings
	{
		public ITimeoutPolicySettings OverallTimeoutPolicySettings { get; set; }
		public ITimeoutPolicySettings TimeoutPerTryPolicySettings { get; set; }
		public IRetrySettings RetrySettings { get; set; }
		public ICircuitBreakerSettings CircuitBreakerSettings { get; set; }

		public static ResiliencePoliciesSettings Default() =>
			new ResiliencePoliciesSettings
		{
			OverallTimeoutPolicySettings = TimeoutPolicySettings.OverallTimeoutPolicySettings.Default(),
			TimeoutPerTryPolicySettings = TimeoutPolicySettings.TimeoutPerTryPolicySettings.Default(),
			RetrySettings = JitterRetrySettings.Default(),
			CircuitBreakerSettings = HttpClientResiliencePolicies.CircuitBreakerSettings.CircuitBreakerSettings.Default()
		};

	}
}
