using System;
using System.Linq;
using System.Net.Http;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace Dodo.HttpClientExtensions
{
    public class RetrySettings
    {
        public int RetryCount { get; }
        public TimeSpan MedianFirstRetryDelay { get; }
        public Func<int, TimeSpan> SleepDurationProvider { get; }
        public Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnRetry { get; }

        public RetrySettings(int retryCount) : this(retryCount, _defaultSleepDurationProvider(retryCount, _defaultMedianFirstRetryDelay))
        {
        }

        public RetrySettings(int retryCount, TimeSpan medianFirstRetryDelay) : this(retryCount, _defaultSleepDurationProvider(retryCount, medianFirstRetryDelay), _defaultOnRetry)
        {
        }


        public RetrySettings(
            int retryCount,
            TimeSpan medianFirstRetryDelay,
            Action<DelegateResult<HttpResponseMessage>, TimeSpan> onRetry)
        {
            RetryCount = retryCount;
            _defaultSleepDurationProvider(retryCount, medianFirstRetryDelay);
            OnRetry = onRetry;
        }

        public RetrySettings(
            int retryCount,
            Func<int, TimeSpan> sleepDurationProvider) : this(retryCount, sleepDurationProvider, _defaultOnRetry)
        {
        }

        public RetrySettings(
            int retryCount,
            Func<int, TimeSpan> sleepDurationProvider,
            Action<DelegateResult<HttpResponseMessage>, TimeSpan> onRetry)
        {
            RetryCount = retryCount;
            SleepDurationProvider = sleepDurationProvider;
            OnRetry = onRetry;
        }

        public static RetrySettings Default() =>
            new RetrySettings(
                DefaultRetryCount,
                _defaultSleepDurationProvider(DefaultRetryCount, _defaultMedianFirstRetryDelay),
                _defaultOnRetry
            );

        private const int DefaultRetryCount = 5;
        private static readonly TimeSpan _defaultMedianFirstRetryDelay = TimeSpan.FromSeconds(3);

        private static readonly Func<int, TimeSpan, Func<int, TimeSpan>> _defaultSleepDurationProvider =
            (retryCount, medianFirstRetryDelay) => i => Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay, retryCount).ToArray()[i];

        private static readonly Action<DelegateResult<HttpResponseMessage>, TimeSpan> _defaultOnRetry =
            (_, __) => { };
    }
}
