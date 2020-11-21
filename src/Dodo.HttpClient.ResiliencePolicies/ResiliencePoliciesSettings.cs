using System;
using System.Net.Http;
using Dodo.HttpClientResiliencePolicies.CircuitBreakerPolicy;
using Dodo.HttpClientResiliencePolicies.RetryPolicy;
using Polly;

namespace Dodo.HttpClientResiliencePolicies
{
	public sealed class ResiliencePoliciesSettings
	{
		private RetryPolicySettings _retryPolicySettings = new RetryPolicySettings();
		private CircuitBreakerPolicySettings _circuitBreakerPolicySettings = new CircuitBreakerPolicySettings();

		public TimeSpan OverallTimeout { get; set; } = TimeSpan.FromMilliseconds(Defaults.Timeout.TimeoutOverallInMilliseconds);
		public TimeSpan TimeoutPerTry { get; set; }= TimeSpan.FromMilliseconds(Defaults.Timeout.TimeoutPerTryInMilliseconds);

		public RetryPolicySettings RetryPolicySettings
		{
			get => _retryPolicySettings;
			set
			{
				var onRetryHandler = OnRetry;

				_retryPolicySettings = value ?? throw new ArgumentNullException(
					$"{nameof(RetryPolicySettings)} cannot be set to null.");
				_retryPolicySettings.OnRetry = onRetryHandler;
			}
		}

		public CircuitBreakerPolicySettings CircuitBreakerPolicySettings
		{
			get => _circuitBreakerPolicySettings;
			set
			{
				var onBreakHandler = OnBreak;
				var onResetHandler = OnReset;
				var onHalfOpenHandler = OnHalfOpen;

				_circuitBreakerPolicySettings = value ?? throw new ArgumentNullException(
					$"{nameof(CircuitBreakerPolicySettings)} cannot be set to null.");
				_circuitBreakerPolicySettings.OnBreak = onBreakHandler;
				_circuitBreakerPolicySettings.OnReset = onResetHandler;
				_circuitBreakerPolicySettings.OnHalfOpen = onHalfOpenHandler;
			}
		}

		public Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnRetry
		{
			get => RetryPolicySettings.OnRetry;
			set => RetryPolicySettings.OnRetry = value;
		}

		public Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnBreak
		{
			get => CircuitBreakerPolicySettings.OnBreak;
			set => CircuitBreakerPolicySettings.OnBreak = value;
		}

		public Action OnReset
		{
			get => CircuitBreakerPolicySettings.OnReset;
			set => CircuitBreakerPolicySettings.OnReset = value;
		}

		public Action OnHalfOpen
		{
			get => CircuitBreakerPolicySettings.OnHalfOpen;
			set => CircuitBreakerPolicySettings.OnHalfOpen = value;
		}
	}
}
