using System;
using System.Net.Http;
using Polly;

namespace Dodo.HttpClientResiliencePolicies.CircuitBreakerPolicy
{
	public sealed class CircuitBreakerPolicySettings
	{
		public double FailureThreshold { get; }
		public int MinimumThroughput { get; }
		public TimeSpan DurationOfBreak { get; }
		public TimeSpan SamplingDuration { get; }

		internal Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnBreak { get; set; }
		internal Action OnReset { get; set; }
		internal Action OnHalfOpen { get; set; }
		internal Func<HttpResponseMessage, bool> ExtraBreakCondition { get; set; }

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

			OnBreak = DoNothingOnBreak;
			OnReset = DoNothingOnReset;
			OnHalfOpen = DoNothingOnHalfOpen;
			ExtraBreakCondition = BreakConditions.OnTooManyRequests;
		}

		private static readonly Action<DelegateResult<HttpResponseMessage>, TimeSpan> DoNothingOnBreak = (_, __) => { };
		private static readonly Action DoNothingOnReset = () => { };
		private static readonly Action DoNothingOnHalfOpen = () => { };
	}
}
