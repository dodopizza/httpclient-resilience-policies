using System;
using System.Net.Http;
using Polly;

namespace Dodo.HttpClientResiliencePolicies.RetryPolicy
{
	public sealed class RetryPolicySettings
	{
		internal ISleepDurationProvider SleepProvider { get; }

		internal Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnRetry { get; set; }

		public RetryPolicySettings(
			ISleepDurationProvider provider)
		{
			if (provider == null) throw new ArgumentNullException(nameof(provider));

			SleepProvider = provider;
			OnRetry = DoNothingOnRetry;
		}

		public RetryPolicySettings()
			: this(SleepDurationProvider.Jitter(
				Defaults.Retry.RetryCount,
				TimeSpan.FromMilliseconds(Defaults.Retry.MedianFirstRetryDelayInMilliseconds)))
		{
		}

		private static readonly Action<DelegateResult<HttpResponseMessage>, TimeSpan> DoNothingOnRetry = (_, __) => { };

		public static RetryPolicySettings Constant(int retryCount)
		{
			return Constant(retryCount,
				TimeSpan.FromMilliseconds(Defaults.Retry.InitialDelayMilliseconds));
		}

		public static RetryPolicySettings Constant(int retryCount, TimeSpan initialDelay)
		{
			return new RetryPolicySettings(
				SleepDurationProvider.Constant(retryCount,initialDelay));
		}

		public static RetryPolicySettings Linear(int retryCount)
		{
			return Linear(retryCount,
				TimeSpan.FromMilliseconds(Defaults.Retry.InitialDelayMilliseconds));
		}

		public static RetryPolicySettings Linear(int retryCount, TimeSpan initialDelay)
		{
			return new RetryPolicySettings(
				SleepDurationProvider.Linear(retryCount, initialDelay));
		}

		public static RetryPolicySettings Exponential(int retryCount)
		{
			return Exponential(retryCount,
				TimeSpan.FromMilliseconds(Defaults.Retry.InitialDelayMilliseconds));
		}

		public static RetryPolicySettings Exponential(int retryCount, TimeSpan initialDelay)
		{
			return new RetryPolicySettings(
				SleepDurationProvider.Exponential(retryCount, initialDelay));
		}

		public static RetryPolicySettings Jitter()
		{
			return Jitter(Defaults.Retry.RetryCount,
				TimeSpan.FromMilliseconds(Defaults.Retry.MedianFirstRetryDelayInMilliseconds));
		}

		public static RetryPolicySettings Jitter(int retryCount)
		{
			return Jitter(retryCount,
				TimeSpan.FromMilliseconds(Defaults.Retry.MedianFirstRetryDelayInMilliseconds));
		}

		public static RetryPolicySettings Jitter(int retryCount, TimeSpan medianFirstRetryDelay)
		{
			return new RetryPolicySettings(
				SleepDurationProvider.Jitter(retryCount, medianFirstRetryDelay));
		}
	}
}
