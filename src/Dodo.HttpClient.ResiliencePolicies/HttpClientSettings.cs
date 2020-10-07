using System;
using Dodo.HttpClient.ResiliencePolicies.CircuitBreakerSettings;
using Dodo.HttpClient.ResiliencePolicies.FallbackSettings;
using Dodo.HttpClient.ResiliencePolicies.RetrySettings;

namespace Dodo.HttpClient.ResiliencePolicies
{
	public class HttpClientSettings
	{
		public TimeSpan HttpClientTimeout { get; }
		public TimeSpan TimeoutPerTry { get; }
		public IFallbackSettings FallbackSettings { get; }
		public IRetrySettings RetrySettings { get; }
		public ICircuitBreakerSettings CircuitBreakerSettings { get; }

		public HttpClientSettings(
			TimeSpan httpClientTimeout,
			TimeSpan timeoutPerTry,
			int retryCount) : this(httpClientTimeout, timeoutPerTry,
			new JitterRetrySettings(retryCount),
			ResiliencePolicies.CircuitBreakerSettings.CircuitBreakerSettings.Default())
		{
		}

		public HttpClientSettings(
			IRetrySettings retrySettings,
			ICircuitBreakerSettings circuitBreakerSettings) : this(
			TimeSpan.FromMilliseconds(Defaults.Timeout.HttpClientTimeoutInMilliseconds),
			TimeSpan.FromMilliseconds(Defaults.Timeout.TimeoutPerTryInMilliseconds),
			retrySettings,
			circuitBreakerSettings)
		{
		}

		public HttpClientSettings(
			IRetrySettings retrySettings,
			ICircuitBreakerSettings circuitBreakerSettings,
			IFallbackSettings fallbackSettings = null) : this(
			TimeSpan.FromMilliseconds(Defaults.Timeout.HttpClientTimeoutInMilliseconds),
			TimeSpan.FromMilliseconds(Defaults.Timeout.TimeoutPerTryInMilliseconds),
			retrySettings,
			circuitBreakerSettings,
			fallbackSettings)
		{
		}

		public HttpClientSettings(
			TimeSpan httpClientTimeout,
			TimeSpan timeoutPerTry,
			IRetrySettings retrySettings,
			ICircuitBreakerSettings circuitBreakerSettings,
			IFallbackSettings fallbackSettings = null)
		{
			HttpClientTimeout = httpClientTimeout;
			TimeoutPerTry = timeoutPerTry;
			FallbackSettings = fallbackSettings;
			RetrySettings = retrySettings;
			CircuitBreakerSettings = circuitBreakerSettings;
		}

		public static HttpClientSettings Default() =>
			new HttpClientSettings(
				JitterRetrySettings.Default(),
				ResiliencePolicies.CircuitBreakerSettings.CircuitBreakerSettings.Default()
			);
	}
}
