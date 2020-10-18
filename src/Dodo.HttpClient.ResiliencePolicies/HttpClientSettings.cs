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
			TimeSpan timeoutOverall,
			TimeSpan timeoutPerTry,
			int retryCount
			) : this(
				timeoutOverall,
				timeoutPerTry,
				new JitterRetrySettings(retryCount),
				HttpClientResiliencePolicies.CircuitBreakerSettings.CircuitBreakerSettings.Default()
			)
		{
		}

		public HttpClientSettings(
			IRetrySettings retrySettings,
			ICircuitBreakerSettings circuitBreakerSettings) : this(
				TimeSpan.FromMilliseconds(Defaults.Timeout.TimeoutOverallInMilliseconds),
				TimeSpan.FromMilliseconds(Defaults.Timeout.TimeoutPerTryInMilliseconds),
				retrySettings,
				circuitBreakerSettings
			)
		{
		}

		public HttpClientSettings(
			TimeSpan timeoutOverall,
			TimeSpan timeoutPerTry,
			IRetrySettings retrySettings,
			ICircuitBreakerSettings circuitBreakerSettings
			)
		{
			TimeoutOverall = timeoutOverall;
			TimeoutPerTry = timeoutPerTry;
			RetrySettings = retrySettings;
			CircuitBreakerSettings = circuitBreakerSettings;
		}

		public static HttpClientSettings Default() =>
			new HttpClientSettings(
				JitterRetrySettings.Default(),
				HttpClientResiliencePolicies.CircuitBreakerSettings.CircuitBreakerSettings.Default()
			);
	}
}
