using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;

namespace Dodo.HttpClientResiliencePolicies.RetryPolicy
{
	internal sealed class RetryPolicyHandler
	{
		private readonly RetryPolicySettings _retryPolicySettings;

		internal RetryPolicyHandler(RetryPolicySettings retryPolicySettings)
		{
			_retryPolicySettings = retryPolicySettings;
		}

		public int RetryCount => _retryPolicySettings.SleepProvider.RetryCount;

		public TimeSpan SleepDurationProvider(int retryCount, DelegateResult<HttpResponseMessage> response, Context context)
		{
			var serverWaitDuration = GetServerWaitDuration(response);
			// ReSharper disable once PossibleMultipleEnumeration
			return serverWaitDuration ?? _retryPolicySettings.SleepProvider.Durations.ToArray()[retryCount-1];
		}

		public Task OnRetry(DelegateResult<HttpResponseMessage> response, TimeSpan span, int retryCount, Context context)
		{
			_retryPolicySettings.OnRetry?.Invoke(response, span);
			// TODO: Async method turned into sync one here
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
