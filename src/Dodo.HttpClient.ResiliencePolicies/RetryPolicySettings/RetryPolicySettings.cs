using System;
using System.Net.Http;
using Polly;

namespace Dodo.HttpClientResiliencePolicies.RetrySettings
{
	public class RetryPolicySettings : IRetryPolicySettings
	{
		public ISleepDurationProvider SleepDurationProvider { get; }
		public Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnRetry { get; set; }

		public RetryPolicySettings()
		: this(RetrySettings.SleepDurationProvider.Jitter(
			Defaults.Retry.RetryCount,
			TimeSpan.FromMilliseconds(Defaults.Retry.MedianFirstRetryDelayInMilliseconds)))
		{
		}

		public RetryPolicySettings(
			ISleepDurationProvider sleepDurationProvider)
		{
			SleepDurationProvider = sleepDurationProvider;
			OnRetry = _doNothingOnRetry;
		}

		private static readonly Action<DelegateResult<HttpResponseMessage>, TimeSpan> _doNothingOnRetry = (_, __) => { };
	}
}
