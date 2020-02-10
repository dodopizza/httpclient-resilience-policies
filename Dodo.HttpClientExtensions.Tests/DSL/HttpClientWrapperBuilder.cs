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
		private TimeSpan _totalTimeout = TimeSpan.FromDays(1);
		private TimeSpan _timeoutPerRequest = TimeSpan.FromDays(1);

		public HttpClientWrapperBuilder WithStatusCode(HttpStatusCode statusCode)
		{
			_statusCode = statusCode;
			return this;
		}

		public HttpClientWrapperBuilder WithTotalTimeout(TimeSpan totalTimeout)
		{
			_totalTimeout = totalTimeout;
			return this;
		}

		public HttpClientWrapperBuilder WithTimeoutPerRequest(TimeSpan timeoutPerRequest)
		{
			_timeoutPerRequest = timeoutPerRequest;
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
				.AddHttpClient(ClientName, c =>
				{
					c.Timeout = _timeoutPerRequest;
				})
				.AddDefaultPolicies(BuildClientSettings())
				.ConfigurePrimaryHttpMessageHandler(() => handler);

			var serviceProvider = services.BuildServiceProvider();
			 var factory = serviceProvider.GetService<IHttpClientFactory>();
			 var client = factory.CreateClient(ClientName);
			 return new HttpClientWrapper(client, handler);
		}

		private ClientSettings BuildClientSettings()
		{
			var defaultCircuitBreakerSettings = _circuitBreakerSettings ?? new CircuitBreakerSettings(
				failureThreshold: 0.5,
				minimumThroughput: int.MaxValue,
				durationOfBreak: TimeSpan.FromMilliseconds(1),
				samplingDuration: TimeSpan.FromMilliseconds(20)
			);

			return new ClientSettings(
				_totalTimeout,
				_timeoutPerRequest,
				_retrySettings ?? JitterRetrySettings.Default(),
				defaultCircuitBreakerSettings);
		}
	}
}
