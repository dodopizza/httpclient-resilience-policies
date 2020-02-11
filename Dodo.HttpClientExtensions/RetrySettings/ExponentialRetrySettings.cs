using System;
using System.Net.Http;
using Polly;

namespace Dodo.HttpClientExtensions
{
	public class ExponentialRetrySettings : IRetrySettings
	{
		public int RetryCount { get; }
		public Func<int, TimeSpan> SleepDurationProvider { get; }
		public Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnRetry { get; }

		public ExponentialRetrySettings(int retryCount) : this(retryCount, _defaultSleepDurationProvider)
		{
		}

		public ExponentialRetrySettings(
			int retryCount,
			Func<int, TimeSpan> sleepDurationProvider) : this(retryCount, sleepDurationProvider, _defaultOnRetry)
		{
		}

		public ExponentialRetrySettings(
			int retryCount,
			Action<DelegateResult<HttpResponseMessage>, TimeSpan> onRetry): this(retryCount, _defaultSleepDurationProvider, onRetry)
		{
		}

		public ExponentialRetrySettings(
			int retryCount,
			Func<int, TimeSpan> sleepDurationProvider,
			Action<DelegateResult<HttpResponseMessage>, TimeSpan> onRetry)
		{
			RetryCount = retryCount;
			SleepDurationProvider = sleepDurationProvider;
			OnRetry = onRetry;
		}

		public static IRetrySettings Default() =>
			new ExponentialRetrySettings(
				DefaultRetryCount,
				_defaultSleepDurationProvider,
				_defaultOnRetry
			);

		private const int DefaultRetryCount = 3;

		private static readonly Func<int, TimeSpan> _defaultSleepDurationProvider =
			i => TimeSpan.FromMilliseconds(20 * Math.Pow(2, i));

		private static readonly Action<DelegateResult<HttpResponseMessage>, TimeSpan> _defaultOnRetry = (_, __) => { };
	}
}
