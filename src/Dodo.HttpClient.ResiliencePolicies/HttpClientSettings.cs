using System;
using Dodo.HttpClientResiliencePolicies.CircuitBreakerSettings;
using Dodo.HttpClientResiliencePolicies.RetrySettings;

namespace Dodo.HttpClientResiliencePolicies
{
	public class HttpClientSettings
	{
		public TimeSpan TimeoutPerTry { get; }
		public TimeSpan TimeoutOverall { get; }
		public IRetrySettings RetrySettings { get; }
		public ICircuitBreakerSettings CircuitBreakerSettings { get; }

		public HttpClientSettings(
			TimeSpan timeoutPerTry,
			int retryCount,
			TimeSpan timeoutOverall) : this(timeoutPerTry,
			new JitterRetrySettings(retryCount),
			HttpClientResiliencePolicies.CircuitBreakerSettings.CircuitBreakerSettings.Default(),
			timeoutOverall)
		{
		}

		public HttpClientSettings(
			IRetrySettings retrySettings,
			ICircuitBreakerSettings circuitBreakerSettings) : this(
			TimeSpan.FromMilliseconds(Defaults.Timeout.TimeoutPerTryInMilliseconds),
			retrySettings,
			circuitBreakerSettings,
			TimeSpan.FromMilliseconds(Defaults.Timeout.TimeoutOverallInMillicesons))
		{
		}

		public HttpClientSettings(
			TimeSpan timeoutPerTry,
			IRetrySettings retrySettings,
			ICircuitBreakerSettings circuitBreakerSettings,
			TimeSpan timeoutOverall)
		{
			TimeoutPerTry = timeoutPerTry;
			RetrySettings = retrySettings;
			CircuitBreakerSettings = circuitBreakerSettings;
			TimeoutOverall = timeoutOverall;
		}

		public static HttpClientSettings Default() =>
			new HttpClientSettings(
				JitterRetrySettings.Default(),
				HttpClientResiliencePolicies.CircuitBreakerSettings.CircuitBreakerSettings.Default()
			);
	}
}
