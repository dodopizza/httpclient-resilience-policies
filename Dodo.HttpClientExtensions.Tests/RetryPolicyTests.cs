using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using Polly;

namespace Dodo.HttpClientExtensions.Tests
{
	[TestFixture]
	public class RetryPolicyTests
	{
		[Test]
		public async Task Should_retry_3_times_when_client_returns_503()
		{
			const int retryCount = 3;
			var retrySettings = new ExponentialRetrySettings(retryCount);
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
			var retrySettings = new JitterRetrySettings(retryCount);
			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithStatusCode(HttpStatusCode.ServiceUnavailable)
				.WithRetrySettings(retrySettings)
				.Please();

			const int taskCount = 2;
			InvokeMultipleHttpRequests(wrapper.Client, taskCount);

			Assert.AreEqual((retryCount + 1) * taskCount, wrapper.NumberOfCalls);
		}

		[Test]
		public async Task Should_separately_distribute_retry_attempts_for_multiple_tasks()
		{
			const int retryCount = 3;
			var retryAttempts = new Dictionary<string, List<TimeSpan>>();
			var retrySettings = new JitterRetrySettings(retryCount, BuildOnRetryAction(retryAttempts));
			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithStatusCode(HttpStatusCode.ServiceUnavailable)
				.WithRetrySettings(retrySettings)
				.Please();

			const int taskCount = 2;
			InvokeMultipleHttpRequests(wrapper.Client, taskCount);

			CollectionAssert.AreNotEquivalent(retryAttempts.First().Value, retryAttempts.Last().Value);
		}

		private static void InvokeMultipleHttpRequests(HttpClient client, int taskCount)
		{
			var tasks = new Task[taskCount];
			for (var i = 0; i < taskCount; i++)
			{
				var requestMessage = new HttpRequestMessage(HttpMethod.Get, "http://localhost");
				requestMessage.Headers.Add("TaskId", i.ToString());
				tasks[i] = client.SendAsync(requestMessage);
			}

			Task.WaitAll(tasks);
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
					retryAttempts[taskId] = new List<TimeSpan> {span};
				}
			};
		}
	}
}
