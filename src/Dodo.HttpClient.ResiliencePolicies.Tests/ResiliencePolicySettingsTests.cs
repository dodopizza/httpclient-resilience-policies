using System;
using System.Net;
using System.Threading.Tasks;
using Dodo.HttpClientResiliencePolicies.CircuitBreakerPolicy;
using Dodo.HttpClientResiliencePolicies.RetryPolicy;
using Dodo.HttpClientResiliencePolicies.Tests.DSL;
using NUnit.Framework;
using Polly.CircuitBreaker;

namespace Dodo.HttpClientResiliencePolicies.Tests
{
	[TestFixture]
	public class ResiliencePolicySettingsTests
	{
		[Test]
		public async Task Should_catch_retry_in_OnRetry_handler_passed_after_RetryPolicySettings()
		{
			var retryCounter = 0;
			var settings = new ResiliencePoliciesSettings
			{
				RetryPolicySettings = RetryPolicySettings.Constant(Defaults.Retry.RetryCount),
				OnRetry = (_, __) => { retryCounter++; },
			};
			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithStatusCode(HttpStatusCode.ServiceUnavailable)
				.WithFullResiliencePolicySettings(settings)
				.Please();

			await wrapper.Client.GetAsync("http://localhost");

			Assert.AreEqual(Defaults.Retry.RetryCount, retryCounter);
		}

		[Test]
		public async Task Should_catch_retry_in_OnRetry_handler_passed_before_RetryPolicySettings()
		{
			var retryCounter = 0;
			var settings = new ResiliencePoliciesSettings
			{
				OnRetry = (_, __) => { retryCounter++; },
				RetryPolicySettings = RetryPolicySettings.Constant(Defaults.Retry.RetryCount),
			};
			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithStatusCode(HttpStatusCode.ServiceUnavailable)
				.WithFullResiliencePolicySettings(settings)
				.Please();

			await wrapper.Client.GetAsync("http://localhost");

			Assert.AreEqual(Defaults.Retry.RetryCount, retryCounter);
		}

		[Test]
		public void Should_catch_CircuitBreaker_OnBreak_handler_passed_through_ResiliencePolicySettings()
		{
			var onBreakFired = false;
			var settings = new ResiliencePoliciesSettings
			{
				CircuitBreakerPolicySettings = BuildCircuitBreakerSettings(),
				OnBreak = (_, __) => { onBreakFired = true; },
			};
			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithStatusCode(HttpStatusCode.ServiceUnavailable)
				.WithFullResiliencePolicySettings(settings)
				.Please();

			const int taskCount = 4;
			Assert.CatchAsync<BrokenCircuitException>(async () =>
				await Helper.InvokeMultipleHttpRequests(wrapper.Client, taskCount));

			Assert.IsTrue(onBreakFired);
		}

		private static ICircuitBreakerPolicySettings BuildCircuitBreakerSettings()
		{
			return new CircuitBreakerPolicySettings(
				failureThreshold: 0.5,
				minimumThroughput: 2,
				durationOfBreak: TimeSpan.FromMinutes(1),
				samplingDuration: TimeSpan.FromMilliseconds(20)
			);
		}
	}
}
