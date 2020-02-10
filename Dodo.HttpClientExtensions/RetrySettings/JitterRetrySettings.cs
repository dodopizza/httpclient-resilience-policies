using System;
using System.Linq;
using System.Net.Http;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace Dodo.HttpClientExtensions
{
    public class JitterRetrySettings : IRetrySettings
    {
        public int RetryCount { get; }
        public TimeSpan MedianFirstRetryDelay { get; }
        public Func<int, TimeSpan> SleepDurationProvider { get; }
        public Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnRetry { get; }

        public JitterRetrySettings(int retryCount) : this(retryCount, _defaultMedianFirstRetryDelay)
        {
        }

        public JitterRetrySettings(int retryCount, TimeSpan medianFirstRetryDelay) : this(retryCount,
            medianFirstRetryDelay, _defaultOnRetry)
        {
        }

        public JitterRetrySettings(
            int retryCount,
            TimeSpan medianFirstRetryDelay,
            Action<DelegateResult<HttpResponseMessage>, TimeSpan> onRetry)
        {
            RetryCount = retryCount;
            SleepDurationProvider = _defaultSleepDurationProvider(retryCount, medianFirstRetryDelay);
            OnRetry = onRetry;
        }

        public static IRetrySettings Default() =>
            new JitterRetrySettings(
                DefaultRetryCount,
                _defaultMedianFirstRetryDelay,
                _defaultOnRetry
            );

        private const int DefaultRetryCount = 5;
        private static readonly TimeSpan _defaultMedianFirstRetryDelay = TimeSpan.FromSeconds(3);

        private static readonly Func<int, TimeSpan, Func<int, TimeSpan>> _defaultSleepDurationProvider =
            (retryCount, medianFirstRetryDelay) => i =>
                Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay, retryCount).ToArray()[i];

        private static readonly Action<DelegateResult<HttpResponseMessage>, TimeSpan> _defaultOnRetry = (_, __) => { };
    }
}
