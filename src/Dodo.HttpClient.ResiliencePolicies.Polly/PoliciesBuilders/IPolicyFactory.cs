using System;
using System.Net.Http;
using Dodo.HttpClientResiliencePolicies.CircuitBreakerPolicy;
using Dodo.HttpClientResiliencePolicies.RetryPolicy;
using Dodo.HttpClientResiliencePolicies.TimeoutPolicy;
using Polly;
using Polly.Retry;
using Polly.Timeout;

namespace Dodo.HttpClientResiliencePolicies.Polly.PoliciesBuilders
{
	internal interface IPolicyFactory
	{
		Func<HttpRequestMessage, IAsyncPolicy<HttpResponseMessage>>
			Create(ICircuitBreakerPolicySettings settings);

		AsyncRetryPolicy<HttpResponseMessage>
			Create(IRetryPolicySettings settings);

		AsyncTimeoutPolicy<HttpResponseMessage>
			Create(ITimeoutPolicySettings settings);
	}
}
