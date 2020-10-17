using System;
using Dodo.HttpClientResiliencePolicies.CircuitBreakerSettings;
using Dodo.HttpClientResiliencePolicies.RetrySettings;

namespace Dodo.HttpClientResiliencePolicies
{
	public class ResiliencePoliciesSettings
	{
		public TimeSpan HttpClientTimeout { get; }
		public TimeSpan TimeoutPerTry { get; }
		public IRetrySettings RetrySettings { get; }
		public ICircuitBreakerSettings CircuitBreakerSettings { get; }


		public ResiliencePoliciesSettings(
			TimeSpan httpClientTimeout,
			TimeSpan timeoutPerTry,
			int retryCount) : this(httpClientTimeout, timeoutPerTry,
			new JitterRetrySettings(retryCount),
			HttpClientResiliencePolicies.CircuitBreakerSettings.CircuitBreakerSettings.Default())
		{
		}

		public ResiliencePoliciesSettings(
			IRetrySettings retrySettings,
			ICircuitBreakerSettings circuitBreakerSettings) : this(
			TimeSpan.FromMilliseconds(Defaults.Timeout.HttpClientTimeoutInMilliseconds),
			TimeSpan.FromMilliseconds(Defaults.Timeout.TimeoutPerTryInMilliseconds),
			retrySettings,
			circuitBreakerSettings)
		{
		}

		public ResiliencePoliciesSettings(
			TimeSpan httpClientTimeout,
			TimeSpan timeoutPerTry,
			IRetrySettings retrySettings,
			ICircuitBreakerSettings circuitBreakerSettings)
		{
			HttpClientTimeout = httpClientTimeout;
			TimeoutPerTry = timeoutPerTry;
			RetrySettings = retrySettings;
			CircuitBreakerSettings = circuitBreakerSettings;
		}

		public static ResiliencePoliciesSettings Default() =>
			new ResiliencePoliciesSettings(
				JitterRetrySettings.Default(),
				HttpClientResiliencePolicies.CircuitBreakerSettings.CircuitBreakerSettings.Default()
			);
	}
}
