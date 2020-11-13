using System;
using System.Net.Http;
using Dodo.HttpClientResiliencePolicies.CircuitBreakerPolicy;
using Dodo.HttpClientResiliencePolicies.RetryPolicy;
using Dodo.HttpClientResiliencePolicies.TimeoutPolicy;
using Polly;
using Polly.Retry;
using Polly.Timeout;

namespace Dodo.HttpClient.ResiliencePolicies.Polly.PoliciesBuilders
{
	public class PolicyFactory : IPolicyFactory
	{
		public Func<HttpRequestMessage, IAsyncPolicy<HttpResponseMessage>>
			Create(ICircuitBreakerPolicySettings settings)
		{
			return new CircleBreakerPolicyBuilder().Build(settings);
		}

		public AsyncRetryPolicy<HttpResponseMessage> Create(IRetryPolicySettings settings)
		{
			return new RetryPolicyBuilder().Build(settings);
		}

		public AsyncTimeoutPolicy<HttpResponseMessage> Create(ITimeoutPolicySettings settings)
		{
			return new TimeoutPolicyBuilder().Build(settings);
		}
	}
}
