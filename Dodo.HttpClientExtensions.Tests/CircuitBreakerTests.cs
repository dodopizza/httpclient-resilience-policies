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
			const int minimumThroughput = 2;
			var retrySettings = new ExponentialRetrySettings(
				retryCount: 5,
				sleepDurationProvider: i => TimeSpan.FromMilliseconds(50));
			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithStatusCode(HttpStatusCode.ServiceUnavailable)
				.WithTotalTimeout(TimeSpan.FromSeconds(5))
				.WithCircuitBreakerSettings(BuildCircuitBreakerSettings(minimumThroughput))
				.WithRetrySettings(retrySettings)
				.Please();

			const int taskCount = 4;
			Assert.CatchAsync<BrokenCircuitException>(async () =>
				await Helper.InvokeMultipleHttpRequests(wrapper.Client, taskCount));

			Assert.AreEqual(minimumThroughput, wrapper.NumberOfCalls);
		}

		private static ICircuitBreakerSettings BuildCircuitBreakerSettings(int throughput)
		{
			return new CircuitBreakerSettings(
				failureThreshold: 0.5,
				minimumThroughput: throughput,
				durationOfBreak: TimeSpan.FromMinutes(1),
				samplingDuration: TimeSpan.FromMilliseconds(20), (_, __) => { }, () => { }, () => { Console.WriteLine("HO");});
		}
	}
}
