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
		public Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnBreak { get; }
		public Action OnReset { get; }
		public Action OnHalfOpen { get; }

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
				DefaultFailureThreshold,
				DefaultMinimumThroughput,
				TimeSpan.FromSeconds(DefaultDurationOfBreakInSec),
				TimeSpan.FromSeconds(DefaultSamplingDurationInSec),
				_defaultOnBreak,
				_defaultOnReset,
				_defaultOnHalfOpen
			);

		private static readonly Action<DelegateResult<HttpResponseMessage>, TimeSpan> _defaultOnBreak = (_, __) => { };
		private static readonly Action _defaultOnReset = () => { };
		private static readonly Action _defaultOnHalfOpen = () => { };
		private const double DefaultFailureThreshold = 0.5;
		private const int DefaultMinimumThroughput = 10;
		private const int DefaultDurationOfBreakInSec = 5;
		private const int DefaultSamplingDurationInSec = 30;
	}
}
