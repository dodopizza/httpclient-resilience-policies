using System;
using Dodo.HttpClientResiliencePolicies.CircuitBreakerPolicy;
using Dodo.HttpClientResiliencePolicies.Polly.PoliciesBuilders;
using Dodo.HttpClientResiliencePolicies.RetryPolicy;
using Dodo.HttpClientResiliencePolicies.TimeoutPolicy;
using Microsoft.Extensions.DependencyInjection;
using IPolicyBuilder = Dodo.HttpClientResiliencePolicies.Core.IPolicyBuilder;

namespace Dodo.HttpClientResiliencePolicies.Polly
{
	public sealed class PolicyBuilder : IPolicyBuilder
	{
		private readonly IHttpClientBuilder _httpClientBuilder;
		private readonly IPolicyFactory _policyFactory;

		public PolicyBuilder(IHttpClientBuilder httpClientBuilder)
		{
			if (httpClientBuilder == null)
				throw new ArgumentNullException(nameof(httpClientBuilder));

			_httpClientBuilder = httpClientBuilder;
			_policyFactory = new PolicyFactory();
		}

		public IPolicyBuilder AddPolicy(
			ITimeoutPolicySettings settings)
		{
			_httpClientBuilder.AddPolicyHandler(_policyFactory.Create(settings));
			 return this;
		}

		public IPolicyBuilder AddPolicy(
			ICircuitBreakerPolicySettings settings)
		{
			_httpClientBuilder.AddPolicyHandler(_policyFactory.Create(settings));
			return this;
		}

		public IPolicyBuilder AddPolicy(
			IRetryPolicySettings settings)
		{
			_httpClientBuilder.AddPolicyHandler(_policyFactory.Create(settings));
			return this;
		}
	}
}
