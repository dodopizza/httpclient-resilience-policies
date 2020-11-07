using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Dodo.HttpClientResiliencePolicies.RetryPolicy;
using Dodo.HttpClientResiliencePolicies.Tests.DSL;
using NUnit.Framework;
using Polly;
using Polly.Timeout;

namespace Dodo.HttpClientResiliencePolicies.Tests
{
	[TestFixture]
	public class RetryPolicyTests
	{
		[Test]
		public async Task Should_retry_3_times_when_client_returns_503()
		{
			const int retryCount = 3;
			var retrySettings = RetryPolicySettings.Constant(retryCount, TimeSpan.FromMilliseconds(1));

			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithStatusCode(HttpStatusCode.ServiceUnavailable)
				.WithRetrySettings(retrySettings)
				.Please();

			var result = await wrapper.Client.GetAsync("http://localhost");

			Assert.AreEqual(HttpStatusCode.ServiceUnavailable, result.StatusCode);
			Assert.AreEqual(retryCount + 1, wrapper.NumberOfCalls);
		}

		[Test]
		public async Task Should_retry_6_times_for_two_threads_when_client_returns_503()
		{
			const int retryCount = 3;
			var retrySettings = RetryPolicySettings.Jitter(retryCount,
					medianFirstRetryDelay: TimeSpan.FromMilliseconds(50));
			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithStatusCode(HttpStatusCode.ServiceUnavailable)
				.WithRetrySettings(retrySettings)
				.Please();

			const int taskCount = 2;
			await Helper.InvokeMultipleHttpRequests(wrapper.Client, taskCount);

			Assert.AreEqual((retryCount + 1) * taskCount, wrapper.NumberOfCalls);
		}

		[Test]
		public async Task Should_separately_distribute_retry_attempts_for_multiple_tasks()
		{
			const int retryCount = 3;
			var retryAttempts = new Dictionary<string, List<TimeSpan>>();
			var retrySettings = RetryPolicySettings.Jitter(retryCount,
				medianFirstRetryDelay: TimeSpan.FromMilliseconds(50));
			retrySettings.
				OnRetry = BuildOnRetryAction(retryAttempts);

			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithStatusCode(HttpStatusCode.ServiceUnavailable)
				.WithRetrySettings(retrySettings)
				.Please();

			const int taskCount = 2;
			await Helper.InvokeMultipleHttpRequests(wrapper.Client, taskCount);

			CollectionAssert.AreNotEquivalent(retryAttempts.First().Value, retryAttempts.Last().Value);
		}

		[Test]
		public async Task Should_retry_when_client_returns_500()
		{
			const int retryCount = 3;
			var retrySettings = RetryPolicySettings.Constant(retryCount, TimeSpan.FromMilliseconds(1));

			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithStatusCode(HttpStatusCode.InternalServerError)
				.WithRetrySettings(retrySettings)
				.Please();

			await wrapper.Client.GetAsync("http://localhost");

			Assert.AreEqual(retryCount + 1, wrapper.NumberOfCalls);
		}

		private Action<DelegateResult<HttpResponseMessage>, TimeSpan> BuildOnRetryAction(
			IDictionary<string, List<TimeSpan>> retryAttempts)
		{
			return (result, span) =>
			{
				var taskId = result.Result.RequestMessage.Headers.GetValues("TaskId").First();
				if (retryAttempts.ContainsKey(taskId))
				{
					retryAttempts[taskId].Add(span);
				}
				else
				{
					retryAttempts[taskId] = new List<TimeSpan> { span };
				}
			};
		}

		[Test]
		public async Task Should_retry_sleep_longer_when_RetryAfterDecorator_is_on()
		{
			const int retryCount = 3;
			var retrySettings = RetryPolicySettings.Constant(retryCount);

			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithRetryAfterHeader(TimeSpan.FromSeconds(1))
				.WithStatusCode(HttpStatusCode.InternalServerError)
				.WithRetrySettings(retrySettings)
				.Please();

			var stopWatch = System.Diagnostics.Stopwatch.StartNew();
			await wrapper.Client.GetAsync("http://localhost");
			stopWatch.Stop();

			Assert.LessOrEqual(3.0d, stopWatch.Elapsed.TotalSeconds);
		}

		[Test]
		public void Should_catchTimeout_because_of_overall_less_then_sleepDuration_of_RetryAfterDecorator()
		{
			const int retryCount = 3;
			var retrySettings = RetryPolicySettings.Constant(retryCount);

			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithRetryAfterHeader(TimeSpan.FromSeconds(1))
				.WithStatusCode(HttpStatusCode.InternalServerError)
				.WithRetrySettings(retrySettings)
				.WithTimeoutOverall(TimeSpan.FromSeconds(2))
				.Please();

			Assert.CatchAsync<TimeoutRejectedException>(async () =>
				await wrapper.Client.GetAsync("http://localhost"));
		}
	}
}
