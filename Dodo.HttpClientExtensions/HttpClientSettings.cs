using System;

namespace Dodo.HttpClientExtensions
{
	public class HttpClientSettings
	{
		public const int DefaultHttpClientTimeoutInMilliseconds = 10000;
		public const int DefaultTimeoutPerTryInMilliseconds = 2000;

		public TimeSpan HttpClientTimeout { get; }
		public TimeSpan TimeoutPerTry { get; }
		public IRetrySettings RetrySettings { get; }
		public ICircuitBreakerSettings CircuitBreakerSettings { get; }


		public HttpClientSettings(
			TimeSpan httpClientTimeout,
			TimeSpan timeoutPerTry,
			int retryCount) : this(httpClientTimeout, timeoutPerTry,
			new JitterRetrySettings(retryCount),
			Dodo.HttpClientExtensions.CircuitBreakerSettings.Default())
		{
		}

		public HttpClientSettings(
			IRetrySettings retrySettings,
			ICircuitBreakerSettings circuitBreakerSettings) : this(
			TimeSpan.FromMilliseconds(DefaultHttpClientTimeoutInMilliseconds),
			TimeSpan.FromMilliseconds(DefaultTimeoutPerTryInMilliseconds),
			retrySettings,
			circuitBreakerSettings)
		{
		}

		public HttpClientSettings(
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

		public static HttpClientSettings Default() =>
			new HttpClientSettings(
				TimeSpan.FromMilliseconds(DefaultHttpClientTimeoutInMilliseconds),
				TimeSpan.FromMilliseconds(DefaultTimeoutPerTryInMilliseconds),
				JitterRetrySettings.Default(),
				HttpClientExtensions.CircuitBreakerSettings.Default()
			);
	}
}
