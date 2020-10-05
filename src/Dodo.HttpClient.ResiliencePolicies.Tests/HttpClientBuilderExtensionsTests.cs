using Dodo.HttpClient.ResiliencePolicies.Tests.Fakes;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dodo.HttpClient.ResiliencePolicies.Tests
{
	[TestFixture]
	public class HttpClientBuilderExtensionsTests
	{
		[Test]
		public async Task AddJsonClient_WithNullClientName_ConfiguresDefaultJsonClient()
		{
			// Arrange
			var serviceCollection = new ServiceCollection();

			// Act1
			serviceCollection.AddJsonClient<IMockJsonClient, MockJsonClient>(
				new Uri("http://example.com/"),
				HttpClientSettings.Default());

			var services = serviceCollection.BuildServiceProvider();

			var factory = services.GetRequiredService<IHttpClientFactory>();

			// Act2
			var client = factory.CreateClient(nameof(IMockJsonClient));

			// Assert
			Assert.NotNull(client);
			Assert.AreEqual("http://example.com/", client.BaseAddress.AbsoluteUri);
		}

		[Test]
		public async Task AddJsonClient_WithSpecificClientName_ConfiguresSpecificJsonClient()
		{
			// Arrange
			var serviceCollection = new ServiceCollection();

			// Act1
			serviceCollection.AddJsonClient<IMockJsonClient, MockJsonClient>(
				new Uri("http://example.com/"),
				HttpClientSettings.Default(),
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
