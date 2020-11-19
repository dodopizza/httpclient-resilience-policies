using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Dodo.HttpClientResiliencePolicies.RetryPolicy;
using Dodo.HttpClientResiliencePolicies.Tests.DSL;
using Dodo.HttpClientResiliencePolicies.TimeoutPolicy;
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
			var settings = new ResiliencePoliciesSettings
			{
				RetryPolicySettings = RetryPolicySettings.Constant(retryCount, TimeSpan.FromMilliseconds(1)),
			};
			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithStatusCode(HttpStatusCode.ServiceUnavailable)
				.WithResiliencePolicySettings(settings)
				.Please();

			var result = await wrapper.Client.GetAsync("http://localhost");

			Assert.AreEqual(HttpStatusCode.ServiceUnavailable, result.StatusCode);
			Assert.AreEqual(retryCount + 1, wrapper.NumberOfCalls);
		}

		[Test]
		public async Task Should_retry_6_times_for_two_threads_when_client_returns_503()
		{
			const int retryCount = 3;
			var settings = new ResiliencePoliciesSettings
			{
				RetryPolicySettings = RetryPolicySettings.Jitter(retryCount, TimeSpan.FromMilliseconds(50)),
			};
			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithStatusCode(HttpStatusCode.ServiceUnavailable)
				.WithResiliencePolicySettings(settings)
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
			var settings = new ResiliencePoliciesSettings
			{
				RetryPolicySettings = RetryPolicySettings.Jitter(retryCount, TimeSpan.FromMilliseconds(50)),
				OnRetry = BuildOnRetryAction(retryAttempts),
			};

			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithStatusCode(HttpStatusCode.ServiceUnavailable)
				.WithResiliencePolicySettings(settings)
				.Please();

			const int taskCount = 2;
			await Helper.InvokeMultipleHttpRequests(wrapper.Client, taskCount);

			CollectionAssert.AreNotEquivalent(retryAttempts.First().Value, retryAttempts.Last().Value);
		}

		[Test]
		public async Task Should_retry_when_client_returns_500()
		{
			const int retryCount = 3;
			var settings = new ResiliencePoliciesSettings
			{
				RetryPolicySettings = RetryPolicySettings.Constant(retryCount, TimeSpan.FromMilliseconds(1)),
			};
			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithStatusCode(HttpStatusCode.InternalServerError)
				.WithResiliencePolicySettings(settings)
				.Please();

			await wrapper.Client.GetAsync("http://localhost");

			Assert.AreEqual(retryCount + 1, wrapper.NumberOfCalls);
		}

		[Test]
		public async Task Should_retry_sleep_longer_when_RetryAfterDecorator_is_on()
		{
			const int retryCount = 3;
			var settings = new ResiliencePoliciesSettings
			{
				RetryPolicySettings = RetryPolicySettings.Constant(retryCount),
			};
			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithRetryAfterHeader(TimeSpan.FromSeconds(1))
				.WithStatusCode(HttpStatusCode.InternalServerError)
				.WithResiliencePolicySettings(settings)
				.Please();

			var stopWatch = Stopwatch.StartNew();
			await wrapper.Client.GetAsync("http://localhost");
			stopWatch.Stop();

			Assert.That(3.0d, Is.GreaterThanOrEqualTo(stopWatch.Elapsed.TotalSeconds).Within(0.1));
		}

		[Test]
		public void Should_catchTimeout_because_of_overall_less_then_sleepDuration_of_RetryAfterDecorator()
		{
			const int retryCount = 3;
			var settings = new ResiliencePoliciesSettings
			{
				OverallTimeoutPolicySettings = new TimeoutPolicySettings(TimeSpan.FromSeconds(2)),
				RetryPolicySettings = RetryPolicySettings.Constant(retryCount),
			};
			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithRetryAfterHeader(TimeSpan.FromSeconds(1))
				.WithStatusCode(HttpStatusCode.InternalServerError)
				.WithResiliencePolicySettings(settings)
				.Please();

			Assert.CatchAsync<TimeoutRejectedException>(async () =>
				await wrapper.Client.GetAsync("http://localhost"));
		}

		private static Action<DelegateResult<HttpResponseMessage>, TimeSpan> BuildOnRetryAction(
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
	}
}
