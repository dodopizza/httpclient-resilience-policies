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

		public static ResiliencePoliciesSettings Default()
		{
			return new ResiliencePoliciesSettings
			{
				OverallTimeoutPolicySettings = Core.TimeoutPolicy.OverallTimeoutPolicySettings.Default(),
				TimeoutPerTryPolicySettings = Core.TimeoutPolicy.TimeoutPerTryPolicySettings.Default(),
				RetrySettings = RetryPolicySettings.Default(),
				CircuitBreakerSettings = CircuitBreakerPolicySettings.Default()
			};
		}
	}
}
