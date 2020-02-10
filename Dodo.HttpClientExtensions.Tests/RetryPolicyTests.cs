using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;

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
			var tasks = new Task[taskCount];
			for (var i = 0; i < taskCount; i++)
			{
				tasks[i] = wrapper.Client.GetAsync("http://localhost");
			}
			Task.WaitAll(tasks);

			Assert.AreEqual((retryCount  + 1) * taskCount, wrapper.NumberOfCalls);
		}
	}
}
