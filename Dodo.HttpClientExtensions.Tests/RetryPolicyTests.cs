using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Dodo.HttpClientExtensions.Tests
{
	public class RetryPolicyTests
	{
		[Test]
		public async Task Should_retry_3_times_when_client_returns_503()
		{
			var retryCount = 3;
			var retrySettings = new ExponentialRetrySettings(retryCount);
			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithStatusCode(HttpStatusCode.ServiceUnavailable)
				.WithRetrySettings(retrySettings)
				.Please();

			await wrapper.Client.GetAsync("http://localhost");

			Assert.AreEqual(retryCount + 1, wrapper.NumberOfCalls);
		}
	}
}
