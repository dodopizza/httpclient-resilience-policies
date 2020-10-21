using System;
using System.Net.Http;
using Polly;

namespace Dodo.HttpClientResiliencePolicies.RetrySettings
{
	public interface IRetrySettings
	{
		int RetryCount { get; set; }
		Func<int, TimeSpan> SleepDurationProvider { get; set; }
		Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnRetry { get; set; }
	}
}
