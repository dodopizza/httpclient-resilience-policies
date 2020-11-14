using Dodo.HttpClientResiliencePolicies.Core.CircuitBreakerPolicy;
using Dodo.HttpClientResiliencePolicies.Core.RetryPolicy;
using Dodo.HttpClientResiliencePolicies.Core.TimeoutPolicy;

namespace Dodo.HttpClientResiliencePolicies.Core
{
	public interface IPolicyBuilder
	{
		IPolicyBuilder AddPolicy(
			ITimeoutPolicySettings settings);

		IPolicyBuilder AddPolicy(
			ICircuitBreakerPolicySettings settings);

		IPolicyBuilder AddPolicy(
			IRetryPolicySettings settings);
	}
}
