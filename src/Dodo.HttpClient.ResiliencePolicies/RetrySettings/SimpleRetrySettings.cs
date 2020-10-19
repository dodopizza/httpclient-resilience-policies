using System;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;

namespace Dodo.HttpClientResiliencePolicies.RetrySettings
{
	public class SimpleRetrySettings : IRetrySettings
	{
		public int RetryCount { get; }
		public Func<int, DelegateResult<HttpResponseMessage>, Context, TimeSpan> SleepDurationProvider { get; }
		public Func<DelegateResult<HttpResponseMessage>, TimeSpan, int, Context, Task> OnRetry { get; set; }

		public SimpleRetrySettings(int retryCount) : this(retryCount, _defaultSleepDurationProvider)
		{
		}

		public SimpleRetrySettings(
			int retryCount,
			Func<int, DelegateResult<HttpResponseMessage>, Context, TimeSpan> sleepDurationProvider) : this(retryCount, sleepDurationProvider, _defaultOnRetry)
		{
		}

		public SimpleRetrySettings(
			int retryCount,
			Func<DelegateResult<HttpResponseMessage>, TimeSpan, int, Context, Task> onRetry) : this(retryCount,
			_defaultSleepDurationProvider, onRetry)
		{
		}

		public SimpleRetrySettings(
			int retryCount,
			Func<int, DelegateResult<HttpResponseMessage>, Context, TimeSpan> sleepDurationProvider,
			Func<DelegateResult<HttpResponseMessage>, TimeSpan, int, Context, Task> onRetry)
		{
			RetryCount = retryCount;
			SleepDurationProvider = sleepDurationProvider;
			OnRetry = onRetry;
		}

		public static IRetrySettings Default() => new SimpleRetrySettings(Defaults.Retry.RetryCount);

		private static readonly Func<int, DelegateResult<HttpResponseMessage>, Context, TimeSpan> _defaultSleepDurationProvider =
			(i,r,c) => TimeSpan.FromMilliseconds(20 * Math.Pow(2, i));

		private static readonly Func<DelegateResult<HttpResponseMessage>, TimeSpan, int, Context, Task> _defaultOnRetry = (_, __, ___, ____) => Task.CompletedTask;
	}
}
