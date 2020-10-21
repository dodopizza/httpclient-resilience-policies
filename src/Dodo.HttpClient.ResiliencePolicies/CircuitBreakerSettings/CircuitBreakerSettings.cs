using System;
using System.Net.Http;
using Polly;

namespace Dodo.HttpClientResiliencePolicies.CircuitBreakerSettings
{
	public class CircuitBreakerSettings : ICircuitBreakerSettings
	{
		public CircuitBreakerSettings()
		{
			FailureThreshold = Defaults.CircuitBreaker.FailureThreshold;
			MinimumThroughput = Defaults.CircuitBreaker.MinimumThroughput;
			DurationOfBreak = TimeSpan.FromMilliseconds(Defaults.CircuitBreaker.DurationOfBreakInMilliseconds);
			SamplingDuration = TimeSpan.FromMilliseconds(Defaults.CircuitBreaker.SamplingDurationInMilliseconds);

			OnBreak = _doNothingOnBreak;
			OnReset = _doNothingOnReset;
			OnHalfOpen = _doNothingOnHalfOpen;
		}

		public double FailureThreshold { get; set; }

		public int MinimumThroughput { get; set; }

		public TimeSpan DurationOfBreak { get; set; }

		public TimeSpan SamplingDuration { get; set; }

		public Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnBreak { get; set; }

		public Action OnReset { get; set; }

		public Action OnHalfOpen { get; set; }

		private static readonly Action<DelegateResult<HttpResponseMessage>, TimeSpan> _doNothingOnBreak = (_, __) => { };
		private static readonly Action _doNothingOnReset = () => { };
		private static readonly Action _doNothingOnHalfOpen = () => { };
	}
}
