using System;
using System.Net.Http;
using Polly;

namespace Dodo.HttpClient.ResiliencePolicies.RetrySettings
{
	public interface IRetrySettings
	{
		int RetryCount { get; }
		Func<int, TimeSpan> SleepDurationProvider { get; }
		Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnRetry { get; set; }
	}
}
