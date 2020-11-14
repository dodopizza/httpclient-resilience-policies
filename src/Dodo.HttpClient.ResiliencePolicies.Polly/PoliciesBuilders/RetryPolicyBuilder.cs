using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Dodo.HttpClientResiliencePolicies.RetryPolicy;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using Polly.Timeout;
using PolicyResult = Dodo.HttpClientResiliencePolicies.Core.PolicyResult;

namespace Dodo.HttpClientResiliencePolicies.Polly.PoliciesBuilders
{
	internal sealed class RetryPolicyBuilder
	{
		public AsyncRetryPolicy<HttpResponseMessage>Build(IRetryPolicySettings settings)
		{
			Task OnRetryWrapper(DelegateResult<HttpResponseMessage> response, TimeSpan span, int retryCount, Context context)
			{
				PolicyResult commonResponse = response.Result != null
					? new PolicyResult(response.Result)
					: new PolicyResult(response.Exception);
				settings.OnRetry(commonResponse, span);
				return Task.CompletedTask;
			}

			var sleepDuration= SleepDurationImplementation.Convert(settings.SleepDurationFunction);

			TimeSpan SleepDurationProvider(int retryCount, DelegateResult<HttpResponseMessage> response, Context context)
			{
				var serverWaitDuration = GetServerWaitDuration(response);
				// ReSharper disable once PossibleMultipleEnumeration
				return serverWaitDuration ?? sleepDuration.ToArray()[retryCount-1];
			};

			return HttpPolicyExtensions
				.HandleTransientHttpError()
				.Or<TimeoutRejectedException>()
				.WaitAndRetryAsync(
					settings.SleepDurationFunction.RetryCount,
					SleepDurationProvider,
					OnRetryWrapper);
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
