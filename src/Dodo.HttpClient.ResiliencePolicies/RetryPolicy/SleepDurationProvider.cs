using System;
using System.Collections.Generic;
using Polly.Contrib.WaitAndRetry;

namespace Dodo.HttpClientResiliencePolicies.RetryPolicy
{
	public sealed class SleepDurationProvider : ISleepDurationProvider
	{
		public IEnumerable<TimeSpan> SleepFunction { get; }

		private SleepDurationProvider(IEnumerable<TimeSpan> sleepFunction)
		{
			SleepFunction = sleepFunction ?? throw new ArgumentNullException(nameof(sleepFunction));
		}

		public static ISleepDurationProvider Constant(int retryCount)
		{
			return Constant(retryCount, TimeSpan.FromMilliseconds(Defaults.Retry.InitialDelayMilliseconds));
		}

		public static ISleepDurationProvider Constant(int retryCount, TimeSpan delay)
		{
			if (retryCount < 0) throw new ArgumentOutOfRangeException(nameof(retryCount), retryCount, "should be >= 0");
			if (delay < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(delay), delay, "should be >= 0ms");

			return new SleepDurationProvider(Backoff.ConstantBackoff(delay, retryCount));
		}

		public static ISleepDurationProvider Linear(int retryCount)
		{
			return Linear(retryCount, TimeSpan.FromMilliseconds(Defaults.Retry.InitialDelayMilliseconds));
		}

		public static ISleepDurationProvider Linear(int retryCount, TimeSpan initialDelay)
		{
			if (retryCount < 0) throw new ArgumentOutOfRangeException(nameof(retryCount), retryCount, "should be >= 0");
			if (initialDelay < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(initialDelay), initialDelay, "should be >= 0ms");

			return new SleepDurationProvider(Backoff.LinearBackoff(initialDelay, retryCount));
		}

		public static ISleepDurationProvider Exponential(int retryCount)
		{
			return Exponential(retryCount, TimeSpan.FromMilliseconds(Defaults.Retry.InitialDelayMilliseconds));
		}

		public static ISleepDurationProvider Exponential(int retryCount, TimeSpan initialDelay)
		{
			if (retryCount < 0) throw new ArgumentOutOfRangeException(nameof(retryCount), retryCount, "should be >= 0");
			if (initialDelay < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(initialDelay), initialDelay, "should be >= 0ms");

			return new SleepDurationProvider(Backoff.ExponentialBackoff(initialDelay, retryCount));
		}

		public static ISleepDurationProvider Jitter(int retryCount)
		{
			return Jitter(retryCount, TimeSpan.FromMilliseconds(Defaults.Retry.MedianFirstRetryDelayInMilliseconds));
		}

		public static ISleepDurationProvider Jitter(int retryCount, TimeSpan medianFirstRetryDelay)
		{
			if (retryCount < 0) throw new ArgumentOutOfRangeException(nameof(retryCount), retryCount, "should be >= 0");
			if (medianFirstRetryDelay < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(medianFirstRetryDelay), medianFirstRetryDelay, "should be >= 0ms");

			return new SleepDurationProvider(Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay, retryCount));
		}
	}
}
