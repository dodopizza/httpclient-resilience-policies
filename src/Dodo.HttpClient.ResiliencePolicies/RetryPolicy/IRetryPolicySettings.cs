using System;
using System.Net.Http;
using Polly;

namespace Dodo.HttpClientResiliencePolicies.RetryPolicy
{
	public interface IRetryPolicySettings
	{
		Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnRetry { get; set; }
		ISleepDurationProvider SleepDurationFunction { get; }
	}
}
