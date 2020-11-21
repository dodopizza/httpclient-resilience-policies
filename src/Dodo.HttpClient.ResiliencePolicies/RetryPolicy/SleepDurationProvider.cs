using System;
using System.Linq;
using System.Net.Http;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace Dodo.HttpClientResiliencePolicies.RetryPolicy
{
	public sealed partial class RetryPolicySettings
	{
		private static class SleepDurationProvider
		{
			internal static Func<int, DelegateResult<HttpResponseMessage>, Context, TimeSpan> Constant(int retryCount, TimeSpan delay)
			{
				if (retryCount < 0) throw new ArgumentOutOfRangeException(nameof(retryCount), retryCount, "should be >= 0");
				if (delay < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(delay), delay, "should be >= 0ms");

				return (i, r, c) => Backoff.ConstantBackoff(delay, retryCount).ToArray()[i - 1];
			}

			internal static Func<int, DelegateResult<HttpResponseMessage>, Context, TimeSpan> Linear(int retryCount, TimeSpan initialDelay)
			{
				if (retryCount < 0) throw new ArgumentOutOfRangeException(nameof(retryCount), retryCount, "should be >= 0");
				if (initialDelay < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(initialDelay), initialDelay, "should be >= 0ms");

				return (i, r, c) => Backoff.LinearBackoff(initialDelay, retryCount).ToArray()[i - 1];
			}

			internal static Func<int, DelegateResult<HttpResponseMessage>, Context, TimeSpan> Exponential(int retryCount, TimeSpan initialDelay)
			{
				if (retryCount < 0) throw new ArgumentOutOfRangeException(nameof(retryCount), retryCount, "should be >= 0");
				if (initialDelay < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(initialDelay), initialDelay, "should be >= 0ms");

				return (i, r, c) => Backoff.ExponentialBackoff(initialDelay, retryCount).ToArray()[i - 1];
			}

			internal static Func<int, DelegateResult<HttpResponseMessage>, Context, TimeSpan> Jitter(int retryCount, TimeSpan medianFirstRetryDelay)
			{
				if (retryCount < 0) throw new ArgumentOutOfRangeException(nameof(retryCount), retryCount, "should be >= 0");
				if (medianFirstRetryDelay < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(medianFirstRetryDelay), medianFirstRetryDelay, "should be >= 0ms");

				return (i, r, c) => Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay, retryCount).ToArray()[i - 1];
			}
		}
	}
}
