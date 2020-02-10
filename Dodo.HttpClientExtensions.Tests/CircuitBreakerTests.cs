using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace Dodo.HttpClientExtensions.Tests
{
	[TestFixture]
	public class CircuitBreakerTests
	{
		[Test]
		public void Should_break_after_4_concurrent_calls()
		{
			const int minimumThroughput = 4;
			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithStatusCode(HttpStatusCode.ServiceUnavailable)
				.WithTotalTimeout(TimeSpan.FromSeconds(5))
				.WithCircuitBreakerSettings(BuildCircuitBreakerSettings(minimumThroughput))
				.WithRetrySettings(BuildRetrySettings())
				.Please();

			Assert.CatchAsync<BrokenCircuitException>(async () => await Helper.InvokeMultipleHttpRequests(wrapper.Client, taskCount: 4));

			Assert.AreEqual(minimumThroughput, wrapper.NumberOfCalls);
		}

		private static IRetrySettings BuildRetrySettings()
		{
			return new ExponentialRetrySettings(
				retryCount: 5,
				sleepDurationProvider: i => TimeSpan.FromMilliseconds(1));
		}

		private static ICircuitBreakerSettings BuildCircuitBreakerSettings(int throughput)
		{
			return new CircuitBreakerSettings(
					failureThreshold: 0.5,
					minimumThroughput: throughput,
					durationOfBreak: TimeSpan.FromMinutes(1),
					samplingDuration: TimeSpan.FromMilliseconds(20));
		}
	}
}
