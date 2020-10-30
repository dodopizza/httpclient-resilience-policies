using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace Dodo.HttpClientResiliencePolicies.RetrySettings
{
	public class RetryPolicySettings
	{
		public int RetryCount { get; set; }

		public RetryPolicySettings()
		{
			RetryCount = Defaults.Retry.RetryCount;
			SleepDurationProvider = _simpleSleepDurationProvider;
			OnRetry = _doNothingOnRetry;
		}

		public Func<int, DelegateResult<HttpResponseMessage>, Context, TimeSpan> SleepDurationProvider {
			get => (retryCount, response, context) =>
			{
				if (_useRetryAfter)
				{
					var serverWaitDuration = getServerWaitDuration(response);

					if (serverWaitDuration != null)
					{
						return serverWaitDuration.Value;
					}
				}

				return _sleepDurationProvider(retryCount, response, context);
			};
			set {
				_sleepDurationProvider = value;
			}
		}

		public virtual Func<DelegateResult<HttpResponseMessage>, TimeSpan, int, Context, Task> OnRetry { get; set; }

		public void EnableRetryAfterFeature()
		{
			_useRetryAfter = true;
		}

		public void UseSimpleStrategy()
		{
			_sleepDurationProvider = _simpleSleepDurationProvider;
		}

		public void UseJitterStrategy()
		{
			_sleepDurationProvider = _jitterSleepDurationProvider(RetryCount,
				TimeSpan.FromMilliseconds(Defaults.Retry.MedianFirstRetryDelayInMilliseconds));
		}

		public void UseJitterStrategy(TimeSpan medianFirstRetryDelay)
		{
			_sleepDurationProvider = _jitterSleepDurationProvider(RetryCount, medianFirstRetryDelay);
		}

		private bool _useRetryAfter;
		private Func<int, DelegateResult<HttpResponseMessage>, Context, TimeSpan> _sleepDurationProvider;

		private static TimeSpan? getServerWaitDuration(DelegateResult<HttpResponseMessage> response)
		{
			var retryAfter = response?.Result?.Headers?.RetryAfter;
			if (retryAfter == null)
				return null;

			return retryAfter.Date.HasValue
				? retryAfter.Date.Value - DateTime.UtcNow
				: retryAfter.Delta.GetValueOrDefault(TimeSpan.Zero);
		}

		private static readonly Func<DelegateResult<HttpResponseMessage>, TimeSpan, int, Context, Task> _doNothingOnRetry = (_, __, ___, ____) => Task.CompletedTask;

		private static Func<int, DelegateResult<HttpResponseMessage>, Context, TimeSpan> _simpleSleepDurationProvider
			= (i, r, c) => TimeSpan.FromMilliseconds(20 * Math.Pow(2, i));

		private static Func<int, TimeSpan, Func<int, DelegateResult<HttpResponseMessage>, Context, TimeSpan>> _jitterSleepDurationProvider =
			(retryCount, medianFirstRetryDelay) => (i, r, c) =>
			   Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay, retryCount).ToArray()[i - 1];
	}
}
