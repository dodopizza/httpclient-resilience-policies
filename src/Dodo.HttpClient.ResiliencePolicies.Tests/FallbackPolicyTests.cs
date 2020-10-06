using Dodo.HttpClient.ResiliencePolicies.Tests.DSL;
using NUnit.Framework;
using Polly;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dodo.HttpClient.ResiliencePolicies.Tests
{
	using FallbackSettings = ResiliencePolicies.FallbackSettings.FallbackSettings;

	[TestFixture]
	public class FallbackPolicyTests
	{
		[Test]
		public async Task When_fallback_with_badRequest_then_client_returns_400()
		{
			var fallbackSettings = new FallbackSettings(new HttpResponseMessage()
			{
				StatusCode = HttpStatusCode.BadRequest
			});

			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithStatusCode(HttpStatusCode.ServiceUnavailable)
				.WithFallbackSettings(fallbackSettings)
				.Please();

			var result = await wrapper.Client.GetAsync("http://localhost");

			Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
		}

		[Test]
		public async Task When_client_returns_200_then_fallback_ignored()
		{
			var fallbackSettings = new FallbackSettings(new HttpResponseMessage()
			{
				StatusCode = HttpStatusCode.BadRequest
			});

			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithStatusCode(HttpStatusCode.OK)
				.WithFallbackSettings(fallbackSettings)
				.Please();

			var result = await wrapper.Client.GetAsync("http://localhost");

			Assert.AreNotEqual(HttpStatusCode.BadRequest, result.StatusCode);
			Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
		}

		[Test]
		public async Task Should_fallback_logging_response_when_client_returns_503()
		{
			StringBuilder logContainer = new StringBuilder();
			var fallbackSettings = new FallbackSettings(new HttpResponseMessage()
			{
				StatusCode = HttpStatusCode.BadRequest
			},
			(result) =>
			{
				logContainer.Append($"Fallback: Origin StatusCode = {result.Result.StatusCode}");
				return Task.CompletedTask;
			});

			var wrapper = Create.HttpClientWrapperWrapperBuilder
				.WithStatusCode(HttpStatusCode.ServiceUnavailable)
				.WithFallbackSettings(fallbackSettings)
				.Please();

			var result = await wrapper.Client.GetAsync("http://localhost");

			Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
			Assert.AreEqual($"Fallback: Origin StatusCode = {HttpStatusCode.ServiceUnavailable}", logContainer.ToString());
		}

		[Test]
		public async Task Should_argumentNullException_when_fallbackSettings_have_null_fallbackValue()
		{
			HttpResponseMessage fallbackValue = null;

			Assert.Catch<ArgumentNullException>(() => new FallbackSettings(fallbackValue));
		}

		[Test]
		public async Task Should_argumentNullException_when_fallbackSettings_have_null_fallbackActionAsync()
		{
			Func<CancellationToken, Task<HttpResponseMessage>> fallbackActionAsync = null;

			Assert.Catch<ArgumentNullException>(() => new FallbackSettings(fallbackActionAsync));
		}

		[Test]
		public async Task Should_argumentNullException_when_fallbackSettings_have_null_onFallbackAsync()
		{
			HttpResponseMessage fallbackValue = new HttpResponseMessage();
			Func<DelegateResult<HttpResponseMessage>, Task> onFallbackAsync = null;

			Assert.Catch<ArgumentNullException>(() => new FallbackSettings(fallbackValue, onFallbackAsync));
		}

		[Test]
		public async Task Should_argumentNullException_when_fallbackSettings_have_null_onFallbackAsync_2()
		{
			Func<CancellationToken, Task<HttpResponseMessage>> fallbackActionAsync = (_) => Task.FromResult(new HttpResponseMessage());
			Func<DelegateResult<HttpResponseMessage>, Task> onFallbackAsync = null;

			Assert.Catch<ArgumentNullException>(() => new FallbackSettings(fallbackActionAsync, onFallbackAsync));
		}
	}
}
