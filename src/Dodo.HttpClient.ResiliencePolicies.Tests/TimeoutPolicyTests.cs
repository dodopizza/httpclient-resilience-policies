using System;
using System.Net;
using System.Threading.Tasks;
using Dodo.HttpClientResiliencePolicies.RetrySettings;
using Dodo.HttpClientResiliencePolicies.Tests.DSL;
using NUnit.Framework;
using Polly.Timeout;

namespace Dodo.HttpClientResiliencePolicies.Tests
{
	[TestFixture]
	public class TimeoutPolicyTests
	{
		[Test]
		public void Should_retry_5_times_200_status_code_because_of_per_try_timeout()
		{
			const int retryCount = 5;
			var retrySettings = new SimpleRetrySettings
			{
				RetryCount = retryCount,
				SleepDurationProvider = i => TimeSpan.FromMilliseconds(200)
			};
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
		public void Should_fail_on_HttpClient_timeout()
		{
			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithResponseLatency(TimeSpan.FromMilliseconds(200))
				.WithTimeoutOverall(TimeSpan.FromMilliseconds(100))
				.Please();

			Assert.CatchAsync<TimeoutRejectedException>(async () =>
				await wrapper.Client.GetAsync("http://localhost"));

			Assert.AreEqual(1, wrapper.NumberOfCalls);
		}


		[Test]
		public void Should_fail_on_HttpClient_timeout_with_retry()
		{
			const int retryCount = 5;
			var retrySettings = new SimpleRetrySettings
			{
				RetryCount = retryCount,
				SleepDurationProvider = i => TimeSpan.FromMilliseconds(1)
			};
			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithStatusCode(HttpStatusCode.ServiceUnavailable)
				.WithResponseLatency(TimeSpan.FromMilliseconds(50))
				.WithTimeoutOverall(TimeSpan.FromMilliseconds(100))
				.WithRetrySettings(retrySettings)
				.Please();

			Assert.CatchAsync<TimeoutRejectedException>(async () =>
				await wrapper.Client.GetAsync("http://localhost"));

			Assert.AreEqual(2, wrapper.NumberOfCalls);
		}

		[Test]
		public void Should_catchTimeout_because_of_overall_timeout()
		{
			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithStatusCode(HttpStatusCode.OK)
				.WithResponseLatency(TimeSpan.FromMilliseconds(200))
				.WithTimeoutOverall(TimeSpan.FromMilliseconds(100))
				.Please();

			Assert.CatchAsync<TimeoutRejectedException>(async () =>
				await wrapper.Client.GetAsync("http://localhost"));
		}

		[Test]
		public void Should_catchTimeout_1_times_because_of_overall_timeout_less_than_per_try_timeout()
		{
			var overallTimeout = TimeSpan.FromMilliseconds(100);
			var perTryTimeout = TimeSpan.FromMilliseconds(200);

			var retrySettings = new SimpleRetrySettings
				{
					RetryCount = 5,
					SleepDurationProvider=i => TimeSpan.FromMilliseconds(200)
				};
			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithStatusCode(HttpStatusCode.OK)
				.WithResponseLatency(TimeSpan.FromMilliseconds(300))
				.WithTimeoutPerTry(perTryTimeout)
				.WithTimeoutOverall(overallTimeout) // less
				.WithRetrySettings(retrySettings)
				.Please();

			Assert.CatchAsync<TimeoutRejectedException>(async () =>
				await wrapper.Client.GetAsync("http://localhost"));

			Assert.AreEqual(1, wrapper.NumberOfCalls);
		}

		[Test]
		public void When_overall_timeout_greated_than_summ_perTrials_Should_retry_5_times_200_status_code_because_of_per_try_timeout_and__()
		{
			const int retryCount = 5;
			var perTryTimeout = TimeSpan.FromMilliseconds(100);
			var overallTimeout = TimeSpan.FromSeconds(2);

			var retrySettings = new SimpleRetrySettings
			{
				RetryCount = retryCount,
				SleepDurationProvider = i => TimeSpan.FromMilliseconds(200)
			};
			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithStatusCode(HttpStatusCode.OK)
				.WithResponseLatency(TimeSpan.FromMilliseconds(200))
				.WithTimeoutPerTry(perTryTimeout)
				.WithTimeoutOverall(overallTimeout)
				.WithRetrySettings(retrySettings)
				.Please();

			Assert.CatchAsync<TimeoutRejectedException>(async () =>
				await wrapper.Client.GetAsync("http://localhost"));

			Assert.AreEqual(retryCount + 1, wrapper.NumberOfCalls);
		}

		[Test]
		public void Should_httpClientTimeout_is_overallTimeout_with_delta_1000ms()
		{
			var overallTimeout = TimeSpan.FromMilliseconds(200);

			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithTimeoutOverall(overallTimeout)
				.Please();

			Assert.AreEqual(200 + 1000, wrapper.Client.Timeout.TotalMilliseconds);
		}
	}
}
