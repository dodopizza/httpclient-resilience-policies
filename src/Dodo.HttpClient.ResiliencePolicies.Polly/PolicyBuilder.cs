using System;
using Dodo.HttpClientResiliencePolicies.Core.CircuitBreakerPolicy;
using Dodo.HttpClientResiliencePolicies.Core.RetryPolicy;
using Dodo.HttpClientResiliencePolicies.Core.TimeoutPolicy;
using Dodo.HttpClientResiliencePolicies.Polly.PoliciesBuilders;
using Microsoft.Extensions.DependencyInjection;
using IPolicyBuilder = Dodo.HttpClientResiliencePolicies.Core.IPolicyBuilder;

namespace Dodo.HttpClientResiliencePolicies.Polly
{
	public sealed class PolicyBuilder : IPolicyBuilder
	{
		private readonly IHttpClientBuilder _httpClientBuilder;

		public PolicyBuilder(IHttpClientBuilder httpClientBuilder)
		{
			if (httpClientBuilder == null)
				throw new ArgumentNullException(nameof(httpClientBuilder));

			_httpClientBuilder = httpClientBuilder;
		}

		public IPolicyBuilder AddPolicy(
			ITimeoutPolicySettings settings)
		{
			_httpClientBuilder.AddPolicyHandler(
				new TimeoutPolicyBuilder().Build(settings));
			 return this;
		}

		public IPolicyBuilder AddPolicy(
			ICircuitBreakerPolicySettings settings)
		{
			_httpClientBuilder.AddPolicyHandler(
				new CircleBreakerPolicyBuilder().Build(settings));
			return this;
		}

		public IPolicyBuilder AddPolicy(
			IRetryPolicySettings settings)
		{
			_httpClientBuilder.AddPolicyHandler(
				new RetryPolicyBuilder().Build(settings));
			return this;
		}
	}
}
