using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;

namespace Dodo.HttpClientResiliencePolicies.RetryPolicy
{
	public interface IRetryPolicySettings
	{
		internal int RetryCount { get; }
		internal Func<int, DelegateResult<HttpResponseMessage>, Context, TimeSpan> SleepDurationProvider { get; }
		internal Func<DelegateResult<HttpResponseMessage>, TimeSpan, int, Context, Task> OnRetryForPolly { get; }
		Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnRetry { get; set; }
	}
}
