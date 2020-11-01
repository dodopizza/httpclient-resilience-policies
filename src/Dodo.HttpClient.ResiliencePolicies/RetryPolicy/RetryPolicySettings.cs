using System;
using System.Net.Http;
using Polly;

namespace Dodo.HttpClientResiliencePolicies.RetryPolicy
{
	public class RetryPolicySettings : IRetryPolicySettings
	{
		public ISleepDurationProvider SleepDurationProvider { get; }
		public Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnRetry { get; set; }

		public RetryPolicySettings()
		: this(RetryPolicy.SleepDurationProvider.Jitter(
			Defaults.Retry.RetryCount,
			TimeSpan.FromMilliseconds(Defaults.Retry.MedianFirstRetryDelayInMilliseconds)))
		{
		}

		public RetryPolicySettings(
			ISleepDurationProvider sleepDurationProvider)
		{
			SleepDurationProvider = sleepDurationProvider;
			OnRetry = DoNothingOnRetry;
		}

		private static readonly Action<DelegateResult<HttpResponseMessage>, TimeSpan> DoNothingOnRetry = (_, __) => { };
	}
}
