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
	public class CircuitBreakerTests
	{
		[Test]
		public void Should_break_after_4_concurrent_calls()
		{
			const int retryCount = 5;
			const int minimumThroughput = 2;
			var settings = new ResiliencePoliciesSettings
			{
				OverallTimeout = TimeSpan.FromSeconds(5),
				RetryPolicySettings = RetryPolicySettings.Constant(retryCount, TimeSpan.FromMilliseconds(100)),
				CircuitBreakerPolicySettings = BuildCircuitBreakerSettings(minimumThroughput),
			};
			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithStatusCode(HttpStatusCode.ServiceUnavailable)
				.WithResiliencePolicySettings(settings)
				.Please();

			const int taskCount = 4;
			Assert.CatchAsync<BrokenCircuitException>(async () =>
				await Helper.InvokeMultipleHttpRequests(wrapper.Client, taskCount));

			Assert.LessOrEqual(wrapper.NumberOfCalls, taskCount);
		}

		[Test]
		public async Task Should_Open_Circuit_Breaker_for_RU_and_do_not_affect_EE()
		{
			const int retryCount = 5;
			const int minimumThroughput = 2;
			var settings = new ResiliencePoliciesSettings
			{
				OverallTimeout = TimeSpan.FromSeconds(5),
				RetryPolicySettings =RetryPolicySettings.Constant(retryCount, TimeSpan.FromMilliseconds(50)),
				CircuitBreakerPolicySettings = BuildCircuitBreakerSettings(minimumThroughput),
			};
			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithHostAndStatusCode("ru-prod.com", HttpStatusCode.ServiceUnavailable)
				.WithHostAndStatusCode("ee-prod.com", HttpStatusCode.OK)
				.WithResiliencePolicySettings(settings)
				.Please();

			const int taskCount = 4;
			Assert.CatchAsync<BrokenCircuitException>(async () =>
				await Helper.InvokeMultipleHttpRequests(wrapper.Client, taskCount,
					"http://ru-prod.com/Test1/Test2"));

			await Helper.InvokeMultipleHttpRequests(wrapper.Client, taskCount,
				"http://ee-prod.com/Test3/Test4/Test5");

			Assert.AreEqual(minimumThroughput + taskCount, wrapper.NumberOfCalls);
		}

		[Test]
		public void Should_not_break_on_429()
		{
			const int retryCount = 5;
			const int minimumThroughput = 2;
			var settings = new ResiliencePoliciesSettings
			{
				OverallTimeout = TimeSpan.FromSeconds(5),
				RetryPolicySettings = RetryPolicySettings.Constant(retryCount, TimeSpan.FromMilliseconds(100)),
				CircuitBreakerPolicySettings = BuildCircuitBreakerSettings(minimumThroughput),
				ExtraBreakCondition = BreakConditions.None
			};
			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithStatusCode(HttpStatusCode.TooManyRequests)
				.WithResiliencePolicySettings(settings)
				.Please();

			const int taskCount = 4;
			Assert.DoesNotThrowAsync(async () =>
				await Helper.InvokeMultipleHttpRequests(wrapper.Client, taskCount));

			Assert.AreEqual(wrapper.NumberOfCalls, taskCount);
		}

		private static CircuitBreakerPolicySettings BuildCircuitBreakerSettings(int throughput)
		{
			return new CircuitBreakerPolicySettings(
				failureThreshold: 0.5,
				minimumThroughput: throughput,
				durationOfBreak: TimeSpan.FromMinutes(1),
				samplingDuration: TimeSpan.FromMilliseconds(20)
			);
		}
	}
}
