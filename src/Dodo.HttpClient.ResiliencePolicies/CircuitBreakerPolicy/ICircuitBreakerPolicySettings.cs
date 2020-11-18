using System;
using System.Net.Http;
using Polly;

namespace Dodo.HttpClientResiliencePolicies.CircuitBreakerPolicy
{
	public interface ICircuitBreakerPolicySettings
	{
		double FailureThreshold { get; }
		int MinimumThroughput { get; }
		TimeSpan DurationOfBreak { get; }
		TimeSpan SamplingDuration { get; }

		internal Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnBreak { get; set; }
		internal Action OnReset { get; set; }
		internal Action OnHalfOpen { get; set; }
	}
}
