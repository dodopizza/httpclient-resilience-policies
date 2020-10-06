using Polly;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dodo.HttpClient.ResiliencePolicies.FallbackSettings
{
	public interface IFallbackSettings
	{
		Func<CancellationToken, Task<HttpResponseMessage>> FallbackActionAsync { get; }

		Func<DelegateResult<HttpResponseMessage>, Task> OnFallbackAsync { get; }
	}
}
