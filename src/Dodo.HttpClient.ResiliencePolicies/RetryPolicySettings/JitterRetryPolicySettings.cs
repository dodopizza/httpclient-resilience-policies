using System;
using System.Linq;
using System.Net.Http;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace Dodo.HttpClientResiliencePolicies.RetrySettings
{
	public class JitterRetryPolicySettings : IRetryPolicySettings
	{
		public int RetryCount { get; set; }
		public Func<int, TimeSpan> SleepDurationProvider { get; set; }
		public Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnRetry { get; set; }

		public TimeSpan MedianFirstRetryDelay {
			get => _defaultMedianFirstRetryDelay;
			set {
				_defaultMedianFirstRetryDelay = value;
				SleepDurationProvider = _defaultSleepDurationProvider(RetryCount, value);
			}
		}

		public JitterRetryPolicySettings()
		{
			RetryCount = Defaults.Retry.RetryCount;
			SleepDurationProvider = _defaultSleepDurationProvider(RetryCount, _defaultMedianFirstRetryDelay);
			OnRetry = _doNothingOnRetry;
		}

		private TimeSpan _defaultMedianFirstRetryDelay =
			TimeSpan.FromMilliseconds(Defaults.Retry.MedianFirstRetryDelayInMilliseconds);

		// i - retry attempt
		private static readonly Func<int, TimeSpan, Func<int, TimeSpan>> _defaultSleepDurationProvider =
			(retryCount, medianFirstRetryDelay) => i =>
				Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay, retryCount).ToArray()[i - 1];

		private static readonly Action<DelegateResult<HttpResponseMessage>, TimeSpan> _doNothingOnRetry = (_, __) => { };
	}
}
