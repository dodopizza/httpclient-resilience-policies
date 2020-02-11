using System;

namespace Dodo.HttpClientExtensions
{
	public class HttpClientSettings
	{
		public const int DefaultTotalTimeOutInMilliseconds = 10000;
		public const int DefaultTimeOutPerRequestInMilliseconds = 2000;

		public TimeSpan TotalTimeout { get; }
		public TimeSpan TimeoutPerRequest { get; }
		public IRetrySettings RetrySettings { get; }
		public ICircuitBreakerSettings CircuitBreakerSettings { get; }


		public HttpClientSettings(
			TimeSpan totalTimeout,
			TimeSpan timeoutPerRequest,
			int retryCount) : this(totalTimeout, timeoutPerRequest,
			new JitterRetrySettings(retryCount),
			Dodo.HttpClientExtensions.CircuitBreakerSettings.Default())
		{
		}

		public HttpClientSettings(
			TimeSpan totalTimeout,
			TimeSpan timeoutPerRequest,
			IRetrySettings retrySettings,
			ICircuitBreakerSettings circuitBreakerSettings)
		{
			TotalTimeout = totalTimeout;
			TimeoutPerRequest = timeoutPerRequest;
			RetrySettings = retrySettings;
			CircuitBreakerSettings = circuitBreakerSettings;
		}

		public static HttpClientSettings Default() =>
			new HttpClientSettings(
				TimeSpan.FromMilliseconds(DefaultTotalTimeOutInMilliseconds),
				TimeSpan.FromMilliseconds(DefaultTimeOutPerRequestInMilliseconds),
				JitterRetrySettings.Default(),
				HttpClientExtensions.CircuitBreakerSettings.Default()
			);
	}
}
