using System;
using System.Net;
using Dodo.HttpClientResiliencePolicies.RetryPolicy;
using Dodo.HttpClientResiliencePolicies.Tests.DSL;
using Dodo.HttpClientResiliencePolicies.TimeoutPolicy;
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
			var settings = new ResiliencePoliciesSettings
			{
				TimeoutPerTryPolicySettings = new TimeoutPolicySettings(TimeSpan.FromMilliseconds(100)),
				RetryPolicySettings = RetryPolicySettings.Constant(retryCount, TimeSpan.FromMilliseconds(200)),
			};
			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithStatusCode(HttpStatusCode.OK)
				.WithResponseLatency(TimeSpan.FromMilliseconds(200))
				.WithResiliencePolicySettings(settings)
				.Please();

			Assert.CatchAsync<TimeoutRejectedException>(async () =>
				await wrapper.Client.GetAsync("http://localhost"));

			Assert.AreEqual(retryCount + 1, wrapper.NumberOfCalls);
		}

		[Test]
		public void Should_fail_on_HttpClient_timeout_with_retry()
		{
			const int retryCount = 5;
			var settings = new ResiliencePoliciesSettings
			{
				OverallTimeoutPolicySettings = new TimeoutPolicySettings(TimeSpan.FromMilliseconds(200)),
				RetryPolicySettings = RetryPolicySettings.Constant(retryCount, TimeSpan.FromMilliseconds(1)),
			};
			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithStatusCode(HttpStatusCode.ServiceUnavailable)
				.WithResponseLatency(TimeSpan.FromMilliseconds(100))
				.WithResiliencePolicySettings(settings)
				.Please();

			Assert.CatchAsync<TimeoutRejectedException>(async () =>
				await wrapper.Client.GetAsync("http://localhost"));
			Assert.AreEqual(2, wrapper.NumberOfCalls);
		}

		[Test]
		public void Should_catch_timeout_because_of_overall_timeout()
		{
			var settings = new ResiliencePoliciesSettings
			{
				OverallTimeoutPolicySettings = new TimeoutPolicySettings(TimeSpan.FromMilliseconds(100)),
			};
			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithStatusCode(HttpStatusCode.OK)
				.WithResponseLatency(TimeSpan.FromMilliseconds(200))
				.WithResiliencePolicySettings(settings)
				.Please();

			Assert.CatchAsync<TimeoutRejectedException>(async () =>
				await wrapper.Client.GetAsync("http://localhost"));
			Assert.AreEqual(1, wrapper.NumberOfCalls);
		}

		[Test]
		public void Should_catch_timeout_1_times_because_of_overall_timeout_less_than_per_try_timeout()
		{
			const int retryCount = 5;
			var settings = new ResiliencePoliciesSettings
			{
				OverallTimeoutPolicySettings = new TimeoutPolicySettings(TimeSpan.FromMilliseconds(100)),
				TimeoutPerTryPolicySettings = new TimeoutPolicySettings(TimeSpan.FromMilliseconds(200)),
				RetryPolicySettings = RetryPolicySettings.Constant(retryCount, TimeSpan.FromMilliseconds(200)),
			};
			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithStatusCode(HttpStatusCode.OK)
				.WithResponseLatency(TimeSpan.FromMilliseconds(300))
				.WithResiliencePolicySettings(settings)
				.Please();

			Assert.CatchAsync<TimeoutRejectedException>(async () =>
				await wrapper.Client.GetAsync("http://localhost"));

			Assert.AreEqual(1, wrapper.NumberOfCalls);
		}

		[Test]
		public void Should_set_HttpClient_Timeout_property_to_overall_timeout_plus_delta_1000ms()
		{
			const int overallTimeoutInMilliseconds = 200;
			var settings = new ResiliencePoliciesSettings
			{
				OverallTimeoutPolicySettings =
					new TimeoutPolicySettings(TimeSpan.FromMilliseconds(overallTimeoutInMilliseconds)),
			};
			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithResiliencePolicySettings(settings)
				.Please();

			Assert.AreEqual(overallTimeoutInMilliseconds + 1000, wrapper.Client.Timeout.TotalMilliseconds);
		}
	}
}
