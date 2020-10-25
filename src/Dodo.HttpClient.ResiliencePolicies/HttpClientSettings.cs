using System;
using Dodo.HttpClientResiliencePolicies.CircuitBreakerSettings;
using Dodo.HttpClientResiliencePolicies.RetrySettings;

namespace Dodo.HttpClientResiliencePolicies
{
	public class HttpClientSettings
	{
		public TimeSpan TimeoutOverall { get; set; } = TimeSpan.FromMilliseconds(Defaults.Timeout.TimeoutOverallInMilliseconds);
		public TimeSpan TimeoutPerTry { get; set; } = TimeSpan.FromMilliseconds(Defaults.Timeout.TimeoutPerTryInMilliseconds);
		public IRetrySettings RetrySettings { get; set; } = new SimpleRetrySettings(Defaults.Retry.RetryCount);
		public ICircuitBreakerSettings CircuitBreakerSettings { get; set; }
			= new CircuitBreakerSettings.CircuitBreakerSettings(
				failureThreshold: Defaults.CircuitBreaker.FailureThreshold,
				minimumThroughput: Defaults.CircuitBreaker.MinimumThroughput,
				durationOfBreak: TimeSpan.FromMilliseconds(Defaults.CircuitBreaker.DurationOfBreakInMilliseconds),
				samplingDuration: TimeSpan.FromMilliseconds(Defaults.CircuitBreaker.SamplingDurationInMilliseconds));

		//public HttpClientSettings(
		//	TimeSpan timeoutOverall,
		//	TimeSpan timeoutPerTry,
		//	int retryCount
		//	) : this(
		//		timeoutOverall,
		//		timeoutPerTry,
		//		new JitterRetrySettings(retryCount),
		//		HttpClientResiliencePolicies.CircuitBreakerSettings.CircuitBreakerSettings.Default()
		//	)
		//{
		//}

		//public HttpClientSettings(
		//	IRetrySettings retrySettings,
		//	ICircuitBreakerSettings circuitBreakerSettings) : this(
		//		TimeSpan.FromMilliseconds(Defaults.Timeout.TimeoutOverallInMilliseconds),
		//		TimeSpan.FromMilliseconds(Defaults.Timeout.TimeoutPerTryInMilliseconds),
		//		retrySettings,
		//		circuitBreakerSettings
		//	)
		//{
		//}

		//public HttpClientSettings(
		//	TimeSpan timeoutOverall,
		//	TimeSpan timeoutPerTry,
		//	IRetrySettings retrySettings,
		//	ICircuitBreakerSettings circuitBreakerSettings
		//	)
		//{
		//	TimeoutOverall = timeoutOverall;
		//	TimeoutPerTry = timeoutPerTry;
		//	RetrySettings = retrySettings;
		//	CircuitBreakerSettings = circuitBreakerSettings;
		//}

		//public static HttpClientSettings Default() =>
		//	new HttpClientSettings(
		//		JitterRetrySettings.Default(),
		//		HttpClientResiliencePolicies.CircuitBreakerSettings.CircuitBreakerSettings.Default()
		//	);
	}
}
