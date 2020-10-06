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
			ResiliencePolicies.FallbackSettings.FallbackSettings.Default(),
			new JitterRetrySettings(retryCount),
			ResiliencePolicies.CircuitBreakerSettings.CircuitBreakerSettings.Default())
		{
		}

		public HttpClientSettings(
			IFallbackSettings fallbackSettings,
			IRetrySettings retrySettings,
			ICircuitBreakerSettings circuitBreakerSettings) : this(
			TimeSpan.FromMilliseconds(Defaults.Timeout.HttpClientTimeoutInMilliseconds),
			TimeSpan.FromMilliseconds(Defaults.Timeout.TimeoutPerTryInMilliseconds),
			fallbackSettings,
			retrySettings,
			circuitBreakerSettings)
		{
		}

		public HttpClientSettings(
			TimeSpan httpClientTimeout,
			TimeSpan timeoutPerTry,
			IFallbackSettings fallbackSettings,
			IRetrySettings retrySettings,
			ICircuitBreakerSettings circuitBreakerSettings)
		{
			HttpClientTimeout = httpClientTimeout;
			TimeoutPerTry = timeoutPerTry;
			FallbackSettings = fallbackSettings;
			RetrySettings = retrySettings;
			CircuitBreakerSettings = circuitBreakerSettings;
		}

		public static HttpClientSettings Default() =>
			new HttpClientSettings(
				ResiliencePolicies.FallbackSettings.FallbackSettings.Default(),
				JitterRetrySettings.Default(),
				ResiliencePolicies.CircuitBreakerSettings.CircuitBreakerSettings.Default()
			);
	}
}
