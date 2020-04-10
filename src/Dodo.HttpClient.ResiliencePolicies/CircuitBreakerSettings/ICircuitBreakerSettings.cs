using System;
using System.Net.Http;
using Polly;

namespace Dodo.HttpClient.ResiliencePolicies.CircuitBreakerSettings
{
	public interface ICircuitBreakerSettings
	{
		double FailureThreshold { get; }
		int MinimumThroughput { get; }
		TimeSpan DurationOfBreak { get; }
		TimeSpan SamplingDuration { get; }
		Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnBreak { get; set; }
		Action OnReset { get; set; }
		Action OnHalfOpen { get; set; }
	}
}
