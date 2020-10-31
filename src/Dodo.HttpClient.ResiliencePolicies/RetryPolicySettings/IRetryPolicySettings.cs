using System;
using System.Net.Http;
using Polly;

namespace Dodo.HttpClientResiliencePolicies.RetrySettings
{
	public interface IRetryPolicySettings
	{
		public ISleepDurationProvider SleepDurationProvider { get; }
		public Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnRetry { get; set; }
	}
}
