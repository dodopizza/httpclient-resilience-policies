using System;
using Dodo.HttpClientResiliencePolicies.Core.RetryPolicy.SleepDurationFunctions;

namespace Dodo.HttpClientResiliencePolicies.Core.RetryPolicy
{
	public class RetryPolicySettings : IRetryPolicySettings
	{
		public ISleepDurationFunction SleepDurationFunction { get; }

		public Action<PolicyResult, TimeSpan> OnRetry { get; set; }

		public RetryPolicySettings()
		: this(new JitterFunction(Defaults.Retry.RetryCount,
			TimeSpan.FromMilliseconds(Defaults.Retry.InitialDelayMilliseconds)))
		{
		}

		private RetryPolicySettings(
			ISleepDurationFunction function)
		{
			SleepDurationFunction = function;

			OnRetry = DoNothingOnRetry;
		}

		public static IRetryPolicySettings Constant(int retryCount)
		{
			return Constant(retryCount,
				TimeSpan.FromMilliseconds(Defaults.Retry.InitialDelayMilliseconds));
		}

		public static IRetryPolicySettings Constant(int retry, TimeSpan timeSpan)
		{
			return new RetryPolicySettings(new ConstantFunction(retry, timeSpan));
		}

		public static IRetryPolicySettings Linear(int retryCount)
		{
			return Linear(retryCount,
				TimeSpan.FromMilliseconds(Defaults.Retry.InitialDelayMilliseconds));
		}

		public static IRetryPolicySettings Linear(int retryCount, TimeSpan initialDelay)
		{
			return new RetryPolicySettings(new LinearFunction(retryCount, initialDelay));
		}

		public static IRetryPolicySettings Exponential(int retryCount)
		{
			return Exponential(retryCount,
				TimeSpan.FromMilliseconds(Defaults.Retry.InitialDelayMilliseconds));
		}

		public static IRetryPolicySettings Exponential(int retryCount, TimeSpan initialDelay)
		{
			return new RetryPolicySettings(new ExponentialFunction(retryCount, initialDelay));
		}

		public static IRetryPolicySettings Jitter(int retryCount)
		{
			return Jitter(retryCount,
				TimeSpan.FromMilliseconds(Defaults.Retry.MedianFirstRetryDelayInMilliseconds));
		}

		public static IRetryPolicySettings Jitter(int retryCount, TimeSpan medianFirstRetryDelay)
		{
			return new RetryPolicySettings(new JitterFunction(retryCount, medianFirstRetryDelay));
		}

		private static readonly Action<PolicyResult, TimeSpan> DoNothingOnRetry = (_, __) => { };
	}
}
