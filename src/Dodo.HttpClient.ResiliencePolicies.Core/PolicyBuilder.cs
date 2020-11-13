using Dodo.HttpClientResiliencePolicies.CircuitBreakerPolicy;
using Dodo.HttpClientResiliencePolicies.RetryPolicy;
using Dodo.HttpClientResiliencePolicies.TimeoutPolicy;

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
