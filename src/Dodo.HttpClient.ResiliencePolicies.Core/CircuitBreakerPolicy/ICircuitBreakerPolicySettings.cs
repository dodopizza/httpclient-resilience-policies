using System;
using PolicyResult = Dodo.HttpClientResiliencePolicies.Core.PolicyResult;

namespace Dodo.HttpClientResiliencePolicies.CircuitBreakerPolicy
{
	public interface ICircuitBreakerPolicySettings
	{
		double FailureThreshold { get; }
		int MinimumThroughput { get; }
		TimeSpan DurationOfBreak { get; }
		TimeSpan SamplingDuration { get; }

		Action<PolicyResult, TimeSpan> OnBreak { get; set; }
		Action OnReset { get; set; }
		Action OnHalfOpen { get; set; }
	}
}
