using System;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;

namespace Dodo.HttpClientResiliencePolicies.RetrySettings
{
	public interface IRetrySettings
	{
		int RetryCount { get; }
		Func<int, DelegateResult<HttpResponseMessage>, Context, TimeSpan> SleepDurationProvider { get; }
		Func<DelegateResult<HttpResponseMessage>, TimeSpan, int, Context, Task> OnRetry { get; set; }
	}
}
