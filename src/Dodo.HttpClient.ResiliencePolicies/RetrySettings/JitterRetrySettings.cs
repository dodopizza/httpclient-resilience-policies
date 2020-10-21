using System;
using System.Linq;
using System.Net.Http;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace Dodo.HttpClientResiliencePolicies.RetrySettings
{
	public class JitterRetrySettings : IRetrySettings
	{
		public JitterRetrySettings()
		{
			RetryCount = Defaults.Retry.RetryCount;
			SleepDurationProvider = _defaultSleepDurationProvider(RetryCount, _defaultMedianFirstRetryDelay);

			OnRetry = _doNothingOnRetry;
		}

		public int RetryCount { get; set; }

		public Func<int, TimeSpan> SleepDurationProvider { get; set; }

		public Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnRetry { get; set; }

		private static readonly TimeSpan _defaultMedianFirstRetryDelay =
			TimeSpan.FromMilliseconds(Defaults.Retry.MedianFirstRetryDelayInMilliseconds);

		// i - retry attempt
		//todo bulanova: temporarily make accessor public for test purposes. It will change when the next step of refactoring will be implemented
		public static readonly Func<int, TimeSpan, Func<int, TimeSpan>> _defaultSleepDurationProvider =
			(retryCount, medianFirstRetryDelay) => i =>
				Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay, retryCount).ToArray()[i - 1];

		private static readonly Action<DelegateResult<HttpResponseMessage>, TimeSpan> _doNothingOnRetry = (_, __) => { };
	}
}
