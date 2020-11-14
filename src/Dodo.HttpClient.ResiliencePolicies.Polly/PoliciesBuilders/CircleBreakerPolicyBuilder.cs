using System;
using System.Net;
using System.Net.Http;
using Dodo.HttpClientResiliencePolicies.Core;
using Dodo.HttpClientResiliencePolicies.Core.CircuitBreakerPolicy;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using PolicyResult = Dodo.HttpClientResiliencePolicies.Core.PolicyResult;

namespace Dodo.HttpClientResiliencePolicies.Polly.PoliciesBuilders
{
	internal sealed class CircleBreakerPolicyBuilder : IPolicyBuilder
	{
		private readonly ICircuitBreakerPolicySettings _settings;

		public CircleBreakerPolicyBuilder(ICircuitBreakerPolicySettings settings)
		{
			_settings = settings;
		}

		public IAsyncPolicy<HttpResponseMessage> Build()
		{
			return HttpPolicyExtensions
				.HandleTransientHttpError()
				.Or<TimeoutRejectedException>()
				.OrResult(r => r.StatusCode == (HttpStatusCode) 429) // Too Many Requests
				.AdvancedCircuitBreakerAsync(
					_settings.FailureThreshold,
					_settings.SamplingDuration,
					_settings.MinimumThroughput,
					_settings.DurationOfBreak,
					OnBreak,
					_settings.OnReset,
					_settings.OnHalfOpen);
		}

		private void OnBreak(DelegateResult<HttpResponseMessage> result, TimeSpan s)
		{
			var commonResponse = result.Result != null
				? new PolicyResult(result.Result)
				: new PolicyResult(result.Exception);
			_settings.OnBreak(commonResponse, s);
		}
	}
}
