using System;
using System.Collections.Generic;
using Polly.Contrib.WaitAndRetry;

namespace Dodo.HttpClientResiliencePolicies.RetryPolicy
{
	public sealed class SleepDurationProvider : ISleepDurationProvider
	{
		public int RetryCount { get; }
		public IEnumerable<TimeSpan> Durations { get; }

		public SleepDurationProvider(int retryCount, IEnumerable<TimeSpan> durations)
		{
			if (retryCount < 0) throw new ArgumentOutOfRangeException(nameof(retryCount), retryCount, "should be >= 0");

			Durations = durations ?? throw new ArgumentNullException(nameof(durations));
			RetryCount = retryCount;
		}

		public static SleepDurationProvider Constant(int retryCount, TimeSpan initialDelay)
		{
			if (retryCount < 0) throw new ArgumentOutOfRangeException(nameof(retryCount), retryCount, "should be >= 0");
			if (initialDelay < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(initialDelay), initialDelay, "should be >= 0ms");

			return new SleepDurationProvider(retryCount, Backoff.ConstantBackoff(initialDelay, retryCount));
		}

		public static SleepDurationProvider Linear(int retryCount, TimeSpan initialDelay)
		{
			if (retryCount < 0) throw new ArgumentOutOfRangeException(nameof(retryCount), retryCount, "should be >= 0");
			if (initialDelay < TimeSpan.Zero)
				throw new ArgumentOutOfRangeException(nameof(initialDelay), initialDelay, "should be >= 0ms");

			return new SleepDurationProvider(retryCount, Backoff.LinearBackoff(initialDelay, retryCount));
		}

		public static SleepDurationProvider Exponential(int retryCount, TimeSpan initialDelay)
		{
			if (retryCount < 0) throw new ArgumentOutOfRangeException(nameof(retryCount), retryCount, "should be >= 0");
			if (initialDelay < TimeSpan.Zero)
				throw new ArgumentOutOfRangeException(nameof(initialDelay), initialDelay, "should be >= 0ms");

			return new SleepDurationProvider(retryCount, Backoff.ExponentialBackoff(initialDelay, retryCount));
		}

		public static SleepDurationProvider Jitter(int retryCount, TimeSpan medianFirstRetryDelay)
		{
			if (retryCount < 0) throw new ArgumentOutOfRangeException(nameof(retryCount), retryCount, "should be >= 0");
			if (medianFirstRetryDelay < TimeSpan.Zero)
				throw new ArgumentOutOfRangeException(nameof(medianFirstRetryDelay), medianFirstRetryDelay,
					"should be >= 0ms");

			return new SleepDurationProvider(retryCount, Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay, retryCount));
		}
	}
}
