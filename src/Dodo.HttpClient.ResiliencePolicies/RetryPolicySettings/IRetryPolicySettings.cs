using System;
using System.Net.Http;
using Polly;

namespace Dodo.HttpClientResiliencePolicies.RetrySettings
{
	public interface IRetryPolicySettings
	{
		public int RetryCount { get; set; }
		public Func<int, TimeSpan> SleepDurationProvider { get; set; }
		public Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnRetry { get; set; }
	}
}
