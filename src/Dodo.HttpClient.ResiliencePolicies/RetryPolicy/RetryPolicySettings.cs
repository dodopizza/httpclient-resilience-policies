using System;
using System.Net.Http;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace Dodo.HttpClientResiliencePolicies.RetryPolicy
{
	public class RetryPolicySettings : IRetryPolicySettings
	{
		public ISleepDurationProvider SleepDurationFunction { get; }

		public Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnRetry { get; set; }

		public RetryPolicySettings(
			ISleepDurationProvider function)
		{
			SleepDurationFunction = function;
			OnRetry = DoNothingOnRetry;
		}

		public RetryPolicySettings(){}

		public static RetryPolicySettings Constant(int retryCount)
		{
			return Constant(retryCount,
				TimeSpan.FromMilliseconds(Defaults.Retry.InitialDelayMilliseconds));
		}

		public static RetryPolicySettings Constant(int retryCount, TimeSpan initialDelay)
		{
			if (retryCount < 0) throw new ArgumentOutOfRangeException(nameof(retryCount), retryCount, "should be >= 0");
			if (initialDelay < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(initialDelay), initialDelay, "should be >= 0ms");

			return new RetryPolicySettings(
				new SleepDurationProvider(retryCount, Backoff.ConstantBackoff(initialDelay, retryCount)));
		}

		public static IRetryPolicySettings Linear(int retryCount)
		{
			return Linear(retryCount,
				TimeSpan.FromMilliseconds(Defaults.Retry.InitialDelayMilliseconds));
		}

		public static IRetryPolicySettings Linear(int retryCount, TimeSpan initialDelay)
		{
			if (retryCount < 0) throw new ArgumentOutOfRangeException(nameof(retryCount), retryCount, "should be >= 0");
			if (initialDelay < TimeSpan.Zero)
				throw new ArgumentOutOfRangeException(nameof(initialDelay), initialDelay, "should be >= 0ms");

			return new RetryPolicySettings(
				new SleepDurationProvider(retryCount, Backoff.LinearBackoff(initialDelay, retryCount)));
		}

		public static IRetryPolicySettings Exponential(int retryCount)
		{
			return Exponential(retryCount,
				TimeSpan.FromMilliseconds(Defaults.Retry.InitialDelayMilliseconds));
		}

		public static IRetryPolicySettings Exponential(int retryCount, TimeSpan initialDelay)
		{
			if (retryCount < 0) throw new ArgumentOutOfRangeException(nameof(retryCount), retryCount, "should be >= 0");
			if (initialDelay < TimeSpan.Zero)
				throw new ArgumentOutOfRangeException(nameof(initialDelay), initialDelay, "should be >= 0ms");

			return new RetryPolicySettings(
				new SleepDurationProvider(retryCount, Backoff.ExponentialBackoff(initialDelay, retryCount)));
		}

		public static IRetryPolicySettings Jitter()
		{
			return Jitter(Defaults.Retry.RetryCount,
				TimeSpan.FromMilliseconds(Defaults.Retry.MedianFirstRetryDelayInMilliseconds));
		}

		public static IRetryPolicySettings Jitter(int retryCount)
		{
			return Jitter(retryCount,
				TimeSpan.FromMilliseconds(Defaults.Retry.MedianFirstRetryDelayInMilliseconds));
		}

		public static IRetryPolicySettings Jitter(int retryCount, TimeSpan medianFirstRetryDelay)
		{
			if (retryCount < 0) throw new ArgumentOutOfRangeException(nameof(retryCount), retryCount, "should be >= 0");
			if (medianFirstRetryDelay < TimeSpan.Zero)
				throw new ArgumentOutOfRangeException(nameof(medianFirstRetryDelay), medianFirstRetryDelay,
					"should be >= 0ms");

			return new RetryPolicySettings(
				new SleepDurationProvider(retryCount, Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay, retryCount)));
		}

		private static readonly Action<DelegateResult<HttpResponseMessage>, TimeSpan> DoNothingOnRetry = (_, __) => { };
	}
}
