using System;
using System.Net.Http;
using Dodo.HttpClientResiliencePolicies.Tests.Fakes;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Dodo.HttpClientResiliencePolicies.Tests
{
	[TestFixture]
	public class HttpClientBuilderExtensionsTests
	{
		[Test]
		public void AddJsonClient_WithNullClientName_ConfiguresDefaultJsonClient()
		{
			// Arrange
			var serviceCollection = new ServiceCollection();

			// Act1
			serviceCollection.AddJsonClient<IMockJsonClient, MockJsonClient>(
				new Uri("http://example.com/"),
				new ResiliencePoliciesSettings());

			var services = serviceCollection.BuildServiceProvider();

			var factory = services.GetRequiredService<IHttpClientFactory>();

			// Act2
			var client = factory.CreateClient(nameof(IMockJsonClient));

			// Assert
			Assert.NotNull(client);
			Assert.AreEqual("http://example.com/", client.BaseAddress.AbsoluteUri);
		}

		[Test]
		public void AddJsonClient_WithSpecificClientName_ConfiguresSpecificJsonClient()
		{
			// Arrange
			var serviceCollection = new ServiceCollection();

			// Act1
			serviceCollection.AddJsonClient<IMockJsonClient, MockJsonClient>(
				new Uri("http://example.com/"),
				new ResiliencePoliciesSettings(),
				"specificName");

			var services = serviceCollection.BuildServiceProvider();

			var factory = services.GetRequiredService<IHttpClientFactory>();

			// Act2
			var client = factory.CreateClient("specificName");

			// Assert
			Assert.NotNull(client);
			Assert.AreEqual("http://example.com/", client.BaseAddress.AbsoluteUri);
		}
	}
}
