using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Dodo.HttpClientExtensions.Tests
{
	public sealed class HttpClientWrapperBuilder
	{
		private const string ClientName = "TestClient";
		private HttpStatusCode _statusCode = HttpStatusCode.OK;
		private IRetrySettings _retrySettings;
		private ICircuitBreakerSettings _circuitBreakerSettings;

		public HttpClientWrapperBuilder WithStatusCode(HttpStatusCode statusCode)
		{
			_statusCode = statusCode;
			return this;
		}

		public HttpClientWrapperBuilder WithRetrySettings(IRetrySettings retrySettings)
		{
			_retrySettings = retrySettings;
			return this;
		}

		public HttpClientWrapperBuilder WithCircuitBreakerSettings(ICircuitBreakerSettings circuitBreakerSettings)
		{
			_circuitBreakerSettings = circuitBreakerSettings;
			return this;
		}

		public HttpClientWrapper Please()
		{
			var handler = new MockHttpMessageHandler(_statusCode);
			var services = new ServiceCollection();
			services
				.AddHttpClient(ClientName)
				.AddDefaultPolicies(BuildClientSettings())
				.ConfigurePrimaryHttpMessageHandler(() => handler);

			var serviceProvider = services.BuildServiceProvider();
			 var factory = serviceProvider.GetService<IHttpClientFactory>();
			 var client = factory.CreateClient(ClientName);
			 return new HttpClientWrapper(client, handler);
		}

		private ClientSettings BuildClientSettings()
		{
			return new ClientSettings(
				TimeSpan.FromMilliseconds(ClientSettings.DefaultTotalTimeOutInMilliseconds),
				TimeSpan.FromMilliseconds(ClientSettings.DefaultTimeOutPerRequestInMilliseconds),
				_retrySettings ?? JitterRetrySettings.Default(),
				_circuitBreakerSettings ?? CircuitBreakerSettings.Default());
		}
	}
}
