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
		public Func<DelegateResult<HttpResponseMessage>, TimeSpan, int, Context, Task> OnRetry { get; set; }
	}
}
