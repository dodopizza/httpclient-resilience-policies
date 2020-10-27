using System;
using System.Net.Http;
using Polly;

namespace Dodo.HttpClientResiliencePolicies.CircuitBreakerSettings
{
	public interface ICircuitBreakerPolicySettings
	{
		double FailureThreshold { get; set; }
		int MinimumThroughput { get; set; }
		TimeSpan DurationOfBreak { get; set; }
		TimeSpan SamplingDuration { get; set; }
		Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnBreak { get; set; }
		Action OnReset { get; set; }
		Action OnHalfOpen { get; set; }
		bool IsHostSpecificOn { get; set; }
	}
}
