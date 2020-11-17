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
		public ResiliencePoliciesSettings()
		{
			OverallTimeoutPolicySettings = new OverallTimeoutPolicySettings();
			TimeoutPerTryPolicySettings = new TimeoutPerTryPolicySettings();
			RetryPolicySettings = new RetryPolicySettings();
			CircuitBreakerPolicySettings = new CircuitBreakerPolicySettings();
		}

		public ResiliencePoliciesSettings(
			TimeSpan overallTimeout,
			TimeSpan timeoutPerTry,
			IRetryPolicySettings retryPolicyPolicySettings,
			ICircuitBreakerPolicySettings circuitBreakerPolicyPolicySettings)
		{
			OverallTimeoutPolicySettings = new OverallTimeoutPolicySettings(overallTimeout);
			TimeoutPerTryPolicySettings = new TimeoutPerTryPolicySettings(timeoutPerTry);
			RetryPolicySettings = retryPolicyPolicySettings;
			CircuitBreakerPolicySettings = circuitBreakerPolicyPolicySettings;
		}

		public ITimeoutPolicySettings OverallTimeoutPolicySettings { get; set; }
		public ITimeoutPolicySettings TimeoutPerTryPolicySettings { get; set; }
		public IRetryPolicySettings RetryPolicySettings { get; set; }
		public ICircuitBreakerPolicySettings CircuitBreakerPolicySettings { get; set; }

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
