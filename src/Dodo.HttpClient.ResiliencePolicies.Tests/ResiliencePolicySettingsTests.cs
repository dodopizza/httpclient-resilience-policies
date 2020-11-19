using System.Net;
using System.Threading.Tasks;
using Dodo.HttpClientResiliencePolicies.RetryPolicy;
using Dodo.HttpClientResiliencePolicies.Tests.DSL;
using NUnit.Framework;

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
				.WithResiliencePolicySettings(settings)
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
				.WithResiliencePolicySettings(settings)
				.Please();

			await wrapper.Client.GetAsync("http://localhost");

			Assert.AreEqual(Defaults.Retry.RetryCount, retryCounter);
		}
	}
}
