using System;
using System.Net.Http;
using Dodo.HttpClientResiliencePolicies.Tests.Fakes;
using Dodo.HttpClientResiliencePolicies.TimeoutPolicy;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Dodo.HttpClientResiliencePolicies.Tests
{
	[TestFixture]
	public class HttpClientBuilderExtensionsTests
	{
		[Test]
		public void When_AddJsonClient_WithNullClientName_than_ConfiguresDefaultJsonClient()
		{
			// Arrange
			var serviceCollection = new ServiceCollection();

			// Act1
			serviceCollection.AddJsonClient<IMockJsonClient, MockJsonClient>(
				new Uri("http://example.com/"));

			var services = serviceCollection.BuildServiceProvider();

			var factory = services.GetRequiredService<IHttpClientFactory>();

			// Act2
			var client = factory.CreateClient(nameof(IMockJsonClient));

			// Assert
			Assert.NotNull(client);
			Assert.AreEqual("http://example.com/", client.BaseAddress.AbsoluteUri);
		}

		[Test]
		public void When_AddJsonClient_WithSpecificClientName_than_ConfiguresSpecificJsonClient()
		{
			// Arrange
			var serviceCollection = new ServiceCollection();

			// Act1
			serviceCollection.AddJsonClient<IMockJsonClient, MockJsonClient>(
				new Uri("http://example.com/"),
				"specificName");

			var services = serviceCollection.BuildServiceProvider();

			var factory = services.GetRequiredService<IHttpClientFactory>();

			// Act2
			var client = factory.CreateClient("specificName");

			// Assert
			Assert.NotNull(client);
			Assert.AreEqual("http://example.com/", client.BaseAddress.AbsoluteUri);
		}

		[Test]
		public void When_AddJsonClient_WithSpecificOverallTimeout_than_ConfiguresSpecificJsonClientTimeout()
		{
			// Arrange
			var serviceCollection = new ServiceCollection();
			var overallTimeout = TimeSpan.FromSeconds(300);

			// Act1
			serviceCollection.AddJsonClient<IMockJsonClient, MockJsonClient>(
				new Uri("http://example.com/"),
				new ResiliencePoliciesSettings
				{
					OverallTimeoutPolicySettings = new OverallTimeoutPolicySettings(overallTimeout),
				});

			var services = serviceCollection.BuildServiceProvider();

			var factory = services.GetRequiredService<IHttpClientFactory>();

			// Act2
			var client = factory.CreateClient(nameof(IMockJsonClient));

			// Assert
			Assert.NotNull(client);
			Assert.AreEqual(overallTimeout.Add(TimeSpan.FromMilliseconds(1000)), client.Timeout);
		}

		[Test]
		public void When_AddJsonClient_WithDefaultOverallTimeout_than_DefaultJsonClientTimeout()
		{
			// Arrange
			var serviceCollection = new ServiceCollection();

			// Act1
			serviceCollection.AddJsonClient<IMockJsonClient, MockJsonClient>(
				new Uri("http://example.com/"));

			var services = serviceCollection.BuildServiceProvider();

			var factory = services.GetRequiredService<IHttpClientFactory>();

			// Act2
			var client = factory.CreateClient(nameof(IMockJsonClient));

			// Assert
			Assert.NotNull(client);
			var overallTimeout = TimeSpan.FromMilliseconds(Defaults.Timeout.TimeoutOverallInMilliseconds);
			Assert.AreEqual(overallTimeout.Add(TimeSpan.FromMilliseconds(1000)), client.Timeout);
		}
	}
}
