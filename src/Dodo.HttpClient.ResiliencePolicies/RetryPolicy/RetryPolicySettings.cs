using System;
using System.Collections.Generic;
using System.Net.Http;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace Dodo.HttpClientResiliencePolicies.RetryPolicy
{
	public class RetryPolicySettings : IRetryPolicySettings
	{
		private readonly IEnumerable<TimeSpan> _sleepDurationProvider;
		IEnumerable<TimeSpan> IRetryPolicySettings.SleepDurationProvider => _sleepDurationProvider;

		public Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnRetry { get; set; }

		public RetryPolicySettings()
		{
			_sleepDurationProvider = SleepDurationProvider.Jitter(
				Defaults.Retry.RetryCount,
				TimeSpan.FromMilliseconds(Defaults.Retry.MedianFirstRetryDelayInMilliseconds));

			OnRetry = DoNothingOnRetry;
		}

		private RetryPolicySettings(
			IEnumerable<TimeSpan> sleepDurationProvider)
		{
			if (sleepDurationProvider == null)
				throw new ArgumentNullException(nameof(sleepDurationProvider));

			_sleepDurationProvider = sleepDurationProvider;
			OnRetry = DoNothingOnRetry;
		}

		private static readonly Action<DelegateResult<HttpResponseMessage>, TimeSpan> DoNothingOnRetry = (_, __) => { };

		public static RetryPolicySettings Constant(int retryCount)
		{
			return Constant(retryCount, TimeSpan.FromMilliseconds(Defaults.Retry.InitialDelayMilliseconds));
		}

		public static RetryPolicySettings Constant(int retryCount, TimeSpan delay)
		{
			return new RetryPolicySettings(SleepDurationProvider.Constant(retryCount, delay));
		}

		public static RetryPolicySettings Linear(int retryCount)
		{
			return Linear(retryCount, TimeSpan.FromMilliseconds(Defaults.Retry.InitialDelayMilliseconds));
		}

		public static RetryPolicySettings Linear(int retryCount, TimeSpan initialDelay)
		{
			return new RetryPolicySettings(SleepDurationProvider.Constant(retryCount, initialDelay));
		}

		public static RetryPolicySettings Exponential(int retryCount)
		{
			return Exponential(retryCount, TimeSpan.FromMilliseconds(Defaults.Retry.InitialDelayMilliseconds));
		}

		public static RetryPolicySettings Exponential(int retryCount, TimeSpan initialDelay)
		{
			return new RetryPolicySettings(SleepDurationProvider.Exponential(retryCount, initialDelay));
		}

		public static RetryPolicySettings Jitter(int retryCount)
		{
			return Jitter(retryCount, TimeSpan.FromMilliseconds(Defaults.Retry.MedianFirstRetryDelayInMilliseconds));
		}

		public static RetryPolicySettings Jitter(int retryCount, TimeSpan medianFirstRetryDelay)
		{
			return new RetryPolicySettings(SleepDurationProvider.Jitter(retryCount, medianFirstRetryDelay));
		}

		#region nested class

		private static class SleepDurationProvider
		{
			internal static IEnumerable<TimeSpan> Constant(int retryCount, TimeSpan delay)
			{
				if (retryCount < 0) throw new ArgumentOutOfRangeException(nameof(retryCount), retryCount, "should be >= 0");
				if (delay < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(delay), delay, "should be >= 0ms");

				return Backoff.ConstantBackoff(delay, retryCount);
			}

			internal static IEnumerable<TimeSpan> Linear(int retryCount, TimeSpan initialDelay)
			{
				if (retryCount < 0) throw new ArgumentOutOfRangeException(nameof(retryCount), retryCount, "should be >= 0");
				if (initialDelay < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(initialDelay), initialDelay, "should be >= 0ms");

				return Backoff.LinearBackoff(initialDelay, retryCount);
			}

			internal static IEnumerable<TimeSpan> Exponential(int retryCount, TimeSpan initialDelay)
			{
				if (retryCount < 0) throw new ArgumentOutOfRangeException(nameof(retryCount), retryCount, "should be >= 0");
				if (initialDelay < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(initialDelay), initialDelay, "should be >= 0ms");

				return Backoff.ExponentialBackoff(initialDelay, retryCount);
			}

			internal static IEnumerable<TimeSpan> Jitter(int retryCount, TimeSpan medianFirstRetryDelay)
			{
				if (retryCount < 0) throw new ArgumentOutOfRangeException(nameof(retryCount), retryCount, "should be >= 0");
				if (medianFirstRetryDelay < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(medianFirstRetryDelay), medianFirstRetryDelay, "should be >= 0ms");

				return Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay, retryCount);
			}

			#endregion
		}
	}
}
