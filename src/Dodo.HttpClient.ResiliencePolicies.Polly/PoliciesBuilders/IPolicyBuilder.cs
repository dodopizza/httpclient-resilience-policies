using System;
using System.Net.Http;
using Polly;

namespace Dodo.HttpClientResiliencePolicies.Polly.PoliciesBuilders
{
	internal interface IPolicyBuilder
	{
		IAsyncPolicy<HttpResponseMessage> Build();
	}

	internal interface IRegistryPolicyBuilder
	{
		Func<HttpRequestMessage, IAsyncPolicy<HttpResponseMessage>> Build();
	}
}
