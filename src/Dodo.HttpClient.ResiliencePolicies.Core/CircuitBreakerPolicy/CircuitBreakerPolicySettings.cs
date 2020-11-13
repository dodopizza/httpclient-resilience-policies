using System;
using System.Net.Http;
using PolicyResult = Dodo.HttpClientResiliencePolicies.Core.PolicyResult;

namespace Dodo.HttpClientResiliencePolicies.CircuitBreakerPolicy
{
	public class CircuitBreakerPolicySettings : ICircuitBreakerPolicySettings
	{
		public double FailureThreshold { get; }
		public int MinimumThroughput { get; }
		public TimeSpan DurationOfBreak { get; }
		public TimeSpan SamplingDuration { get; }

		public Action<PolicyResult, TimeSpan>  OnBreak { get; set; }
		public Action OnReset { get; set; }
		public Action OnHalfOpen { get; set; }

		public CircuitBreakerPolicySettings()
			: this(
				Defaults.CircuitBreaker.FailureThreshold,
				Defaults.CircuitBreaker.MinimumThroughput,
				TimeSpan.FromMilliseconds(Defaults.CircuitBreaker.DurationOfBreakInMilliseconds),
				TimeSpan.FromMilliseconds(Defaults.CircuitBreaker.SamplingDurationInMilliseconds))
		{
		}

		public CircuitBreakerPolicySettings(
			double failureThreshold,
			int minimumThroughput,
			TimeSpan durationOfBreak,
			TimeSpan samplingDuration)
		{
			FailureThreshold = failureThreshold;
			MinimumThroughput = minimumThroughput;
			DurationOfBreak = durationOfBreak;
			SamplingDuration = samplingDuration;

			//OnBreak = DoNothingOnBreak;
			OnReset = DoNothingOnReset;
			OnHalfOpen = DoNothingOnHalfOpen;
		}

	//	private static readonly Action<DelegateResult<HttpResponseMessage>, TimeSpan> DoNothingOnBreak = (_, __) => { };
		private static readonly Action DoNothingOnReset = () => { };
		private static readonly Action DoNothingOnHalfOpen = () => { };
	}
}
