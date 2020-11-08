using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;

namespace Dodo.HttpClientResiliencePolicies.RetryPolicy
{
	public interface IRetryPolicySettings
	{
		Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnRetry { get; set; }

		internal int RetryCount { get; }
		internal Func<int, DelegateResult<HttpResponseMessage>, Context, TimeSpan> SleepDurationProvider { get; }
		internal Func<DelegateResult<HttpResponseMessage>, TimeSpan, int, Context, Task> OnRetryWrapper { get; }
	}
}
