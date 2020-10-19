using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace Dodo.HttpClientResiliencePolicies.RetrySettings
{
	public class JitterRetrySettings : IRetrySettings
	{
		public int RetryCount { get; }
		public TimeSpan MedianFirstRetryDelay { get; }
		public Func<int, DelegateResult<HttpResponseMessage>, Context, TimeSpan> SleepDurationProvider { get; }
		public Func<DelegateResult<HttpResponseMessage>, TimeSpan, int, Context, Task> OnRetry { get; set; }

		public JitterRetrySettings(int retryCount) : this(retryCount, _defaultMedianFirstRetryDelay)
		{
		}

		public JitterRetrySettings(int retryCount, Func<DelegateResult<HttpResponseMessage>, TimeSpan, int, Context, Task> onRetry) :
			this(retryCount, _defaultMedianFirstRetryDelay, onRetry)
		{
		}

		public JitterRetrySettings(int retryCount, TimeSpan medianFirstRetryDelay) : this(retryCount,
			medianFirstRetryDelay, _defaultOnRetry)
		{
		}

		public JitterRetrySettings(
			int retryCount,
			TimeSpan medianFirstRetryDelay,
			Func<DelegateResult<HttpResponseMessage>, TimeSpan, int, Context, Task> onRetry)
		{
			RetryCount = retryCount;
			SleepDurationProvider = _defaultSleepDurationProvider(retryCount, medianFirstRetryDelay);
			OnRetry = onRetry;
		}

		public static IRetrySettings Default() => new JitterRetrySettings(Defaults.Retry.RetryCount);

		private static readonly TimeSpan _defaultMedianFirstRetryDelay =
			TimeSpan.FromMilliseconds(Defaults.Retry.MedianFirstRetryDelayInMilliseconds);

		// i - retry attempt
		private static readonly Func<int, TimeSpan, Func<int, DelegateResult<HttpResponseMessage>, Context, TimeSpan>> _defaultSleepDurationProvider =
			(retryCount, medianFirstRetryDelay) => (i,r,c) =>
				Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay, retryCount).ToArray()[i - 1];

		private static readonly Func<DelegateResult<HttpResponseMessage>, TimeSpan, int, Context, Task> _defaultOnRetry = (_, __, ___, ____) => Task.CompletedTask;
	}
}
