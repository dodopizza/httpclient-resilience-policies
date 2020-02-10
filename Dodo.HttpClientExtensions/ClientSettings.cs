using System;

namespace Dodo.HttpClientExtensions
{
    public class ClientSettings
    {
        public TimeSpan TotalTimeout { get; }
        public TimeSpan TimeoutPerRequest { get; }
        public IRetrySettings RetrySettings { get; }
        public ICircuitBreakerSettings CircuitBreakerSettings { get; }

        public ClientSettings(
            TimeSpan totalTimeout,
            TimeSpan timeoutPerRequest,
            int retryCount) : this(totalTimeout, timeoutPerRequest,
            new JitterRetrySettings(retryCount),
            Dodo.HttpClientExtensions.CircuitBreakerSettings.Default())
        {
        }

        public ClientSettings(
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

        public static ClientSettings Default() =>
            new ClientSettings(
                TimeSpan.FromMilliseconds(DefaultTotalTimeOutInMilliseconds),
                TimeSpan.FromMilliseconds(DefaultTimeOutPerRequestInMilliseconds),
                JitterRetrySettings.Default(),
                HttpClientExtensions.CircuitBreakerSettings.Default()
            );

        private const int DefaultTotalTimeOutInMilliseconds = 10000;
        private const int DefaultTimeOutPerRequestInMilliseconds = 2000;
    }
}
