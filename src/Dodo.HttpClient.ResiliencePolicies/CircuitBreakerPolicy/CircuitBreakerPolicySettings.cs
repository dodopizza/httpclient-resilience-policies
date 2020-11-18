using System;
using System.Net.Http;
using Polly;

namespace Dodo.HttpClientResiliencePolicies.CircuitBreakerPolicy
{
	public class CircuitBreakerPolicySettings : ICircuitBreakerPolicySettings
	{
		private Action<DelegateResult<HttpResponseMessage>, TimeSpan> _onBreakHandler;
		private Action _onResetHandler;
		private Action _onHalfOpenHandler;

		public double FailureThreshold { get; }
		public int MinimumThroughput { get; }
		public TimeSpan DurationOfBreak { get; }
		public TimeSpan SamplingDuration { get; }

		Action<DelegateResult<HttpResponseMessage>, TimeSpan> ICircuitBreakerPolicySettings.OnBreak
		{
			get => _onBreakHandler;
			set => _onBreakHandler = value;
		}

		Action ICircuitBreakerPolicySettings.OnReset
		{
			get => _onResetHandler;
			set => _onResetHandler = value;
		}

		Action ICircuitBreakerPolicySettings.OnHalfOpen
		{
			get => _onHalfOpenHandler;
			set => _onHalfOpenHandler = value;
		}

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

			_onBreakHandler = DoNothingOnBreak;
			_onResetHandler = DoNothingOnReset;
			_onHalfOpenHandler = DoNothingOnHalfOpen;
		}

		private static readonly Action<DelegateResult<HttpResponseMessage>, TimeSpan> DoNothingOnBreak = (_, __) => { };
		private static readonly Action DoNothingOnReset = () => { };
		private static readonly Action DoNothingOnHalfOpen = () => { };
	}
}
