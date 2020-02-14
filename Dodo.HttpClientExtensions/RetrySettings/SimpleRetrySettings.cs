using System;
using System.Net.Http;
using Polly;

namespace Dodo.HttpClientExtensions
{
	public class SimpleRetrySettings : IRetrySettings
	{
		public int RetryCount { get; }
		public Func<int, TimeSpan> SleepDurationProvider { get; }
		public Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnRetry { get; set; }

		public SimpleRetrySettings(int retryCount) : this(retryCount, _defaultSleepDurationProvider)
		{
		}

		public SimpleRetrySettings(
			int retryCount,
			Func<int, TimeSpan> sleepDurationProvider) : this(retryCount, sleepDurationProvider, _defaultOnRetry)
		{
		}

		public SimpleRetrySettings(
			int retryCount,
			Action<DelegateResult<HttpResponseMessage>, TimeSpan> onRetry): this(retryCount, _defaultSleepDurationProvider, onRetry)
		{
		}

		public SimpleRetrySettings(
			int retryCount,
			Func<int, TimeSpan> sleepDurationProvider,
			Action<DelegateResult<HttpResponseMessage>, TimeSpan> onRetry)
		{
			RetryCount = retryCount;
			SleepDurationProvider = sleepDurationProvider;
			OnRetry = onRetry;
		}

		public static IRetrySettings Default() =>
			new SimpleRetrySettings(
				Defaults.Retry.RetryCount
			);

		private static readonly Func<int, TimeSpan> _defaultSleepDurationProvider =
			i => TimeSpan.FromMilliseconds(20 * Math.Pow(2, i));

		private static readonly Action<DelegateResult<HttpResponseMessage>, TimeSpan> _defaultOnRetry = (_, __) => { };
	}
}
