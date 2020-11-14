using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Dodo.HttpClientResiliencePolicies.Core.RetryPolicy;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using PolicyResult = Dodo.HttpClientResiliencePolicies.Core.PolicyResult;

namespace Dodo.HttpClientResiliencePolicies.Polly.PoliciesBuilders
{
	internal sealed class RetryPolicyBuilder : IPolicyBuilder
	{
		private readonly int _retryCount;
		private readonly IEnumerable<TimeSpan> _sleepDuration;
		private readonly Action<PolicyResult, TimeSpan> _onRetry;

		public RetryPolicyBuilder(IRetryPolicySettings settings)
		{
			_sleepDuration= SleepDurationImplementation.Convert(settings.SleepDurationFunction);
			_retryCount = settings.SleepDurationFunction.RetryCount;
			_onRetry = settings.OnRetry;
		}

		public IAsyncPolicy<HttpResponseMessage>  Build()
		{
			return HttpPolicyExtensions
				.HandleTransientHttpError()
				.Or<TimeoutRejectedException>()
				.WaitAndRetryAsync(
					_retryCount,
					SleepDurationProvider,
					OnRetry);
		}

		private TimeSpan SleepDurationProvider(int retryCount, DelegateResult<HttpResponseMessage> response, Context context)
		{
			var serverWaitDuration = GetServerWaitDuration(response);
			// ReSharper disable once PossibleMultipleEnumeration
			return serverWaitDuration ?? _sleepDuration.ToArray()[retryCount-1];
		}

		private Task OnRetry(DelegateResult<HttpResponseMessage> response, TimeSpan span, int retryCount, Context context)
		{
			PolicyResult commonResponse = response.Result != null
				? new PolicyResult(response.Result)
				: new PolicyResult(response.Exception);
			_onRetry(commonResponse, span);
			return Task.CompletedTask;
		}

		private static TimeSpan? GetServerWaitDuration(DelegateResult<HttpResponseMessage> response)
		{
			var retryAfter = response?.Result?.Headers?.RetryAfter;
			if (retryAfter == null)
			{
				return null;
			}

			if (retryAfter.Delta.HasValue) // Delta priority check, because its simple TimeSpan value
			{
				return retryAfter.Delta.Value;
			}

			if (retryAfter.Date.HasValue)
			{
				return retryAfter.Date.Value - DateTime.UtcNow;
			}

			return null; // when nothing was found
		}
	}
}
