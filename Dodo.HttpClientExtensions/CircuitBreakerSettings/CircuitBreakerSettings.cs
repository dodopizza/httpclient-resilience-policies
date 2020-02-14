using System;
using System.Net.Http;
using Polly;

namespace Dodo.HttpClientExtensions
{
	public class CircuitBreakerSettings : ICircuitBreakerSettings
	{
		public double FailureThreshold { get; }
		public int MinimumThroughput { get; }
		public TimeSpan DurationOfBreak { get; }
		public TimeSpan SamplingDuration { get; }
		public Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnBreak { get; set; }
		public Action OnReset { get; set; }
		public Action OnHalfOpen { get; set; }

		public CircuitBreakerSettings(
			double failureThreshold,
			int minimumThroughput,
			TimeSpan durationOfBreak,
			TimeSpan samplingDuration) : this(failureThreshold, minimumThroughput, durationOfBreak, samplingDuration,
			_defaultOnBreak, _defaultOnReset, _defaultOnHalfOpen)
		{
		}

		public CircuitBreakerSettings(
			double failureThreshold,
			int minimumThroughput,
			TimeSpan durationOfBreak,
			TimeSpan samplingDuration,
			Action<DelegateResult<HttpResponseMessage>, TimeSpan> onBreak,
			Action onReset,
			Action onHalfOpen)
		{
			FailureThreshold = failureThreshold;
			MinimumThroughput = minimumThroughput;
			DurationOfBreak = durationOfBreak;
			SamplingDuration = samplingDuration;
			OnBreak = onBreak;
			OnReset = onReset;
			OnHalfOpen = onHalfOpen;
		}

		public static ICircuitBreakerSettings Default() =>
			new CircuitBreakerSettings(
				Defaults.CircuitBreaker.FailureThreshold,
				Defaults.CircuitBreaker.MinimumThroughput,
				TimeSpan.FromMilliseconds(Defaults.CircuitBreaker.DurationOfBreakInMilliseconds),
				TimeSpan.FromMilliseconds(Defaults.CircuitBreaker.SamplingDurationInMilliseconds)
			);

		private static readonly Action<DelegateResult<HttpResponseMessage>, TimeSpan> _defaultOnBreak = (_, __) => { };
		private static readonly Action _defaultOnReset = () => { };
		private static readonly Action _defaultOnHalfOpen = () => { };
	}
}
