using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Polly.Timeout;

namespace Dodo.HttpClientExtensions.Tests
{
	[TestFixture]
	public class TimeoutPolicyTests
	{
		[Test]
		public async Task Should_retry_5_times_200_status_code_because_of_per_try_timeout()
		{
			const int retryCount = 5;
			var retrySettings = new ExponentialRetrySettings(
				retryCount,
				sleepDurationProvider: i => TimeSpan.FromMilliseconds(200));
			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithStatusCode(HttpStatusCode.OK)
				.WithResponseLatency(TimeSpan.FromMilliseconds(200))
				.WithTimeoutPerTry(TimeSpan.FromMilliseconds(100))
				.WithRetrySettings(retrySettings)
				.Please();

			Assert.CatchAsync<TimeoutRejectedException>(async () =>
				await wrapper.Client.GetAsync("http://localhost"));

			Assert.AreEqual(retryCount + 1, wrapper.NumberOfCalls);
		}


		[Test]
		public async Task Should_fail_on_HttpClient_timeout()
		{
			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithResponseLatency(TimeSpan.FromMilliseconds(200))
				.WithHttpClientTimeout(TimeSpan.FromMilliseconds(100))
				.Please();

			Assert.CatchAsync<TaskCanceledException>(async () =>
				await wrapper.Client.GetAsync("http://localhost"));

			Assert.AreEqual(1, wrapper.NumberOfCalls);
		}


		[Test]
		public async Task Should_fail_on_HttpClient_timeout_with_retry()
		{
			const int retryCount = 5;
			var retrySettings = new ExponentialRetrySettings(
				retryCount,
				sleepDurationProvider: i => TimeSpan.FromMilliseconds(1));
			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithStatusCode(HttpStatusCode.ServiceUnavailable)
				.WithResponseLatency(TimeSpan.FromMilliseconds(50))
				.WithHttpClientTimeout(TimeSpan.FromMilliseconds(100))
				.WithRetrySettings(retrySettings)
				.Please();

			Assert.CatchAsync<TaskCanceledException>(async () =>
				await wrapper.Client.GetAsync("http://localhost"));

			Assert.AreEqual(2, wrapper.NumberOfCalls);
		}
	}
}
