using System;
using System.Net.Http;
using Dodo.HttpClientResiliencePolicies.CircuitBreakerPolicy;
using Dodo.HttpClientResiliencePolicies.RetryPolicy;
using Dodo.HttpClientResiliencePolicies.TimeoutPolicy;
using Polly;

namespace Dodo.HttpClientResiliencePolicies
{
	public class ResiliencePoliciesSettings
	{
		private ITimeoutPolicySettings _overallTimeoutPolicySettings = new OverallTimeoutPolicySettings();
		private ITimeoutPolicySettings _timeoutPerTryPolicySettings = new TimeoutPerTryPolicySettings();
		private IRetryPolicySettings _retryPolicySettings = new RetryPolicySettings();
		private ICircuitBreakerPolicySettings _circuitBreakerPolicySettings = new CircuitBreakerPolicySettings();

		public ResiliencePoliciesSettings()
		{
		}

		public ResiliencePoliciesSettings(
			TimeSpan overallTimeout,
			TimeSpan timeoutPerTry,
			IRetryPolicySettings retryPolicyPolicySettings,
			ICircuitBreakerPolicySettings circuitBreakerPolicyPolicySettings)
		{
			_overallTimeoutPolicySettings = new OverallTimeoutPolicySettings(overallTimeout);
			_timeoutPerTryPolicySettings = new TimeoutPerTryPolicySettings(timeoutPerTry);
			_retryPolicySettings = retryPolicyPolicySettings;
			_circuitBreakerPolicySettings = circuitBreakerPolicyPolicySettings;
		}

		public ITimeoutPolicySettings OverallTimeoutPolicySettings
		{
			get => _overallTimeoutPolicySettings;
			set => _overallTimeoutPolicySettings = value ?? throw new NullReferenceException(
				$"{nameof(OverallTimeoutPolicySettings)} cannot be set to null.");
		}
		public ITimeoutPolicySettings TimeoutPerTryPolicySettings
		{
			get => _timeoutPerTryPolicySettings;
			set => _timeoutPerTryPolicySettings = value ?? throw new NullReferenceException(
				$"{nameof(TimeoutPerTryPolicySettings)} cannot be set to null.");
		}

		public IRetryPolicySettings RetryPolicySettings
		{
			get => _retryPolicySettings;
			set
			{
				var onRetryHandler = OnRetry;

				_retryPolicySettings = value ?? throw new NullReferenceException(
					$"{nameof(RetryPolicySettings)} cannot be set to null.");
				_retryPolicySettings.OnRetry = onRetryHandler;
			}
		}

		public ICircuitBreakerPolicySettings CircuitBreakerPolicySettings
		{
			get => _circuitBreakerPolicySettings;
			set
			{
				var onBreakHandler = OnBreak;
				var onResetHandler = OnReset;
				var onHalfOpenHandler = OnHalfOpen;

				_circuitBreakerPolicySettings = value ?? throw new NullReferenceException(
					$"{nameof(CircuitBreakerPolicySettings)} cannot be set to null.");
				_circuitBreakerPolicySettings.OnBreak = onBreakHandler;
				_circuitBreakerPolicySettings.OnReset = onResetHandler;
				_circuitBreakerPolicySettings.OnHalfOpen = onHalfOpenHandler;
			}
		}

		public Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnRetry
		{
			get => RetryPolicySettings?.OnRetry;
			set
			{
				if (RetryPolicySettings == null) throw new NullReferenceException(
					$"{nameof(RetryPolicySettings)} should be initialized first.");
				RetryPolicySettings.OnRetry = value;
			}
		}

		public Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnBreak
		{
			get => CircuitBreakerPolicySettings?.OnBreak;
			set
			{
				if (CircuitBreakerPolicySettings == null) throw new NullReferenceException(
					$"{nameof(CircuitBreakerPolicySettings)} should be initialized first.");
				CircuitBreakerPolicySettings.OnBreak = value;
			}
		}

		public Action OnReset
		{
			get => CircuitBreakerPolicySettings?.OnReset;
			set
			{
				if (CircuitBreakerPolicySettings == null) throw new NullReferenceException(
					$"{nameof(CircuitBreakerPolicySettings)} should be initialized first.");
				CircuitBreakerPolicySettings.OnReset = value;
			}
		}

		public Action OnHalfOpen
		{
			get => CircuitBreakerPolicySettings?.OnHalfOpen;
			set
			{
				if (CircuitBreakerPolicySettings == null) throw new NullReferenceException(
					$"{nameof(CircuitBreakerPolicySettings)} should be initialized first.");
				CircuitBreakerPolicySettings.OnHalfOpen = value;
			}
		}
	}
}
