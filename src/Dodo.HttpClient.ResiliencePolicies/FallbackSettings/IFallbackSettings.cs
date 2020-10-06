using Polly;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Dodo.HttpClient.ResiliencePolicies.FallbackSettings
{
	public interface IFallbackSettings
	{
		Action FallbackAction { get; }

		Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnFallback { get; }
	}
}
