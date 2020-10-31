using System;
using System.Net.Http;
using Polly;

namespace Dodo.HttpClientResiliencePolicies.CircuitBreakerSettings
{
	public class CircuitBreakerPolicySettings : ICircuitBreakerPolicySettings
	{
		public double FailureThreshold { get; }
		public int MinimumThroughput { get; }
		public TimeSpan DurationOfBreak { get; }
		public TimeSpan SamplingDuration { get; }

		public Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnBreak { get; set; }
		public Action OnReset { get; set; }
		public Action OnHalfOpen { get; set; }

		public bool IsHostSpecificOn { get; set; }

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

			OnBreak = _doNothingOnBreak;
			OnReset = _doNothingOnReset;
			OnHalfOpen = _doNothingOnHalfOpen;
		}

		private static readonly Action<DelegateResult<HttpResponseMessage>, TimeSpan> _doNothingOnBreak = (_, __) => { };
		private static readonly Action _doNothingOnReset = () => { };
		private static readonly Action _doNothingOnHalfOpen = () => { };
	}
}
