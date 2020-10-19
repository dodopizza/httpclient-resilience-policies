using Polly;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dodo.HttpClientResiliencePolicies.RetrySettings
{
	public class RetryAfterDecorator : IRetrySettings
	{
		public int RetryCount => _decoratedRetrySettings.RetryCount;

		public Func<int, DelegateResult<HttpResponseMessage>, Context, TimeSpan> SleepDurationProvider {
			get => (retryCount, response, context) =>
			{
				var serverWaitDuration = getServerWaitDuration(response);

				if (serverWaitDuration != null)
				{
					return serverWaitDuration.Value;
				}

				return _decoratedRetrySettings.SleepDurationProvider(retryCount, response, context);
			};
		}

		public Func<DelegateResult<HttpResponseMessage>, TimeSpan, int, Context, Task> OnRetry
		{
			get => _decoratedRetrySettings.OnRetry;
			set => _decoratedRetrySettings.OnRetry = value;
		}

		public RetryAfterDecorator(IRetrySettings decoratedRetrySettings)
		{
			_decoratedRetrySettings = decoratedRetrySettings;
		}

		private readonly IRetrySettings _decoratedRetrySettings;

		private static TimeSpan? getServerWaitDuration(DelegateResult<HttpResponseMessage> response)
		{
			var retryAfter = response?.Result?.Headers?.RetryAfter;
			if (retryAfter == null)
				return null;

			return retryAfter.Date.HasValue
				? retryAfter.Date.Value - DateTime.UtcNow
				: retryAfter.Delta.GetValueOrDefault(TimeSpan.Zero);
		}
	}
}
