using System;
using System.Net.Http;
using Polly;
using Polly.Registry;

namespace Dodo.HttpClientResiliencePolicies.Polly.PoliciesBuilders
{
	internal sealed class PolicyRegistryBuilder : IRegistryPolicyBuilder
	{
		private readonly IPolicyBuilder _policyBuilder;

		public PolicyRegistryBuilder(IPolicyBuilder policyBuilder)
		{
			_policyBuilder = policyBuilder;
		}

		public Func<HttpRequestMessage, IAsyncPolicy<HttpResponseMessage>> Build()
		{
			// This implementation takes into consideration situations
			// when you use the only HttpClient against different hosts.
			// In this case we want to have separate CircuitBreaker metrics for each host.
			// It allows us avoid situations when all requests to all hosts
			// will be stopped by CircuitBreaker due to single host is not available.
			var registry = new PolicyRegistry();
			return message =>
			{
				var policyKey = message.RequestUri.Host;
				var policy = registry.GetOrAdd(policyKey, _policyBuilder.Build());
				return policy;
			};
		}
	}
}
