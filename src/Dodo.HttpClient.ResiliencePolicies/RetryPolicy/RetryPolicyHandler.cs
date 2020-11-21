using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;

namespace Dodo.HttpClientResiliencePolicies.RetryPolicy
{
	internal sealed class RetryPolicyHandler
	{
		private readonly IRetryPolicySettings _retryPolicySettings;

		internal RetryPolicyHandler(IRetryPolicySettings retryPolicySettings)
		{
			_retryPolicySettings = retryPolicySettings;
		}

		public int RetryCount => _retryPolicySettings.SleepDurationFunction.RetryCount;

		public TimeSpan SleepDurationProvider(int retryCount, DelegateResult<HttpResponseMessage> response, Context context)
		{
			var serverWaitDuration = GetServerWaitDuration(response);
			// ReSharper disable once PossibleMultipleEnumeration
			return serverWaitDuration ?? _retryPolicySettings.SleepDurationFunction.Durations.ToArray()[retryCount-1];
		}

		public Task OnRetry(DelegateResult<HttpResponseMessage> response, TimeSpan span, int retryCount, Context context)
		{
			_retryPolicySettings.OnRetry(response, span);
			//todo bulanova: не нравится что асихронный метод в синхронный превращается
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
