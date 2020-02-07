using System;

namespace Dodo.HttpClientExtensions
{
    public class ClientSettings
    {
        public TimeSpan TotalTimeout { get; }
        public TimeSpan TimeoutPerRequest { get; }
        public RetrySettings RetrySettings { get; }
        public CircuitBreakerSettings CircuitBreakerSettings { get; }

        public ClientSettings(
            TimeSpan totalTimeout,
            TimeSpan timeoutPerRequest,
            int retryCount) : this(totalTimeout, timeoutPerRequest, new RetrySettings(retryCount),
            CircuitBreakerSettings.Default())
        {
        }

        public ClientSettings(
            TimeSpan totalTimeout,
            TimeSpan timeoutPerRequest,
            RetrySettings retrySettings,
            CircuitBreakerSettings circuitBreakerSettings)
        {
            TotalTimeout = totalTimeout;
            TimeoutPerRequest = timeoutPerRequest;
            RetrySettings = retrySettings;
            CircuitBreakerSettings = circuitBreakerSettings;
        }

        public static ClientSettings Default() =>
            new ClientSettings(
                TimeSpan.FromMilliseconds(DefaultTotalTimeOutInMilliseconds),
                TimeSpan.FromMilliseconds(DefaultTimeOutPerRequestInMilliseconds),
                RetrySettings.Default(),
                CircuitBreakerSettings.Default()
            );

        private const int DefaultTotalTimeOutInMilliseconds = 10000;
        private const int DefaultTimeOutPerRequestInMilliseconds = 2000;
    }
}
