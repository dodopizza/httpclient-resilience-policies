using System;
using System.Linq;
using System.Net.Http;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace Dodo.HttpClientResiliencePolicies.RetrySettings
{
	public class JitterRetryPolicySettings : IRetryPolicySettings
	{
		public int RetryCount { get; }
		public Func<int, TimeSpan> SleepDurationProvider { get; }
		public Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnRetry { get; set; }

		public JitterRetryPolicySettings()
		: this(Defaults.Retry.RetryCount)
		{
		}

		public JitterRetryPolicySettings(int retryCount)
		: this(retryCount,
			TimeSpan.FromMilliseconds(Defaults.Retry.MedianFirstRetryDelayInMilliseconds))
		{
		}

		public JitterRetryPolicySettings(
			int retryCount,
			TimeSpan medianFirstRetryDelay)
		{
			RetryCount = retryCount;
			SleepDurationProvider = _defaultSleepDurationProvider(RetryCount, medianFirstRetryDelay);
			OnRetry = _doNothingOnRetry;
		}

		// i - retry attempt
		private static readonly Func<int, TimeSpan, Func<int, TimeSpan>> _defaultSleepDurationProvider =
			(retryCount, medianFirstRetryDelay) => i =>
				Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay, retryCount).ToArray()[i - 1];

		private static readonly Action<DelegateResult<HttpResponseMessage>, TimeSpan> _doNothingOnRetry = (_, __) => { };
	}
}
