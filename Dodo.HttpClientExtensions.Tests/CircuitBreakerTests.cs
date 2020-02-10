using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Dodo.HttpClientExtensions.Tests
{
	[TestFixture]
	public class CircuitBreakerTests
	{
		[Test]
		public async Task Should_break_after_4_concurrent_calls()
		{
			const int retryCount = 5;
			var retrySettings =
				new ExponentialRetrySettings(retryCount, i => TimeSpan.FromMilliseconds(1));
			long breakCount = 0;
			var circuitBreakerSettings =
				new CircuitBreakerSettings(0.5, 4, TimeSpan.FromMinutes(1),
					TimeSpan.FromMilliseconds(20), (_, __) => { breakCount++; },
					() => { },
					() => { });
			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithStatusCode(HttpStatusCode.ServiceUnavailable)
				.WithTotalTimeout(TimeSpan.FromSeconds(5))
				.WithCircuitBreakerSettings(circuitBreakerSettings)
				.WithRetrySettings(retrySettings)
				.Please();

			const int taskCount = 4;
			try
			{
				await Helper.InvokeMultipleHttpRequests(wrapper.Client, taskCount);
			}
			catch (Polly.Timeout.TimeoutRejectedException)
			{}
			catch (Polly.CircuitBreaker.BrokenCircuitException)
			{}

			Assert.AreEqual(1, breakCount);
			Assert.AreEqual(circuitBreakerSettings.MinimumThroughput, wrapper.NumberOfCalls);
		}
	}
}
