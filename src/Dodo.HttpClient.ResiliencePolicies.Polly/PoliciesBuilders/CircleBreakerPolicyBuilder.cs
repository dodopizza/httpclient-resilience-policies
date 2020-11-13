using System;
using System.Net;
using System.Net.Http;
using Dodo.HttpClientResiliencePolicies.CircuitBreakerPolicy;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Registry;
using Polly.Timeout;
using PolicyResult = Dodo.HttpClientResiliencePolicies.Core.PolicyResult;

namespace Dodo.HttpClient.ResiliencePolicies.Polly.PoliciesBuilders
{
	public class CircleBreakerPolicyBuilder // : IPolicyBuilder<Func<HttpRequestMessage, IAsyncPolicy<HttpResponseMessage>>>
	{

		internal Func<HttpRequestMessage, IAsyncPolicy<HttpResponseMessage>> Build(ICircuitBreakerPolicySettings settings)
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
				var policy = registry.GetOrAdd(policyKey, Build1(settings));
				return policy;
			};
		}

		internal static AsyncCircuitBreakerPolicy<HttpResponseMessage> Build1(
			ICircuitBreakerPolicySettings settings)
		{
			void OnBreakWrapper(DelegateResult<HttpResponseMessage> result, TimeSpan s)
			{
				var commonResponse = result.Result != null
					? new PolicyResult(result.Result)
					: new PolicyResult(result.Exception);
				settings.OnBreak(commonResponse, s);
			}

			return HttpPolicyExtensions
				.HandleTransientHttpError()
				.Or<TimeoutRejectedException>()
				.OrResult(r => r.StatusCode == (HttpStatusCode) 429) // Too Many Requests
				.AdvancedCircuitBreakerAsync(
					settings.FailureThreshold,
					settings.SamplingDuration,
					settings.MinimumThroughput,
					settings.DurationOfBreak,
					OnBreakWrapper,
					settings.OnReset,
					settings.OnHalfOpen);
		}
	}
}
