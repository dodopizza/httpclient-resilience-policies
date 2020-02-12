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
		private TimeSpan _httpClientTimeout = TimeSpan.FromDays(1);
		private TimeSpan _timeoutPerTry = TimeSpan.FromDays(1);

		public HttpClientWrapperBuilder WithStatusCode(HttpStatusCode statusCode)
		{
			_statusCode = statusCode;
			return this;
		}

		public HttpClientWrapperBuilder WithHttpClientTimeout(TimeSpan httpClientTimeout)
		{
			_httpClientTimeout = httpClientTimeout;
			return this;
		}

		public HttpClientWrapperBuilder WithTimeoutPerTry(TimeSpan timeoutPerTry)
		{
			_timeoutPerTry = timeoutPerTry;
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
				.AddHttpClient(ClientName, c => { c.Timeout = _httpClientTimeout; })
				.AddDefaultPolicies(BuildClientSettings())
				.ConfigurePrimaryHttpMessageHandler(() => handler);

			var serviceProvider = services.BuildServiceProvider();
			var factory = serviceProvider.GetService<IHttpClientFactory>();
			var client = factory.CreateClient(ClientName);
			return new HttpClientWrapper(client, handler);
		}

		private HttpClientSettings BuildClientSettings()
		{
			var defaultCircuitBreakerSettings = _circuitBreakerSettings ?? new CircuitBreakerSettings(
				                                    failureThreshold: 0.5,
				                                    minimumThroughput: int.MaxValue,
				                                    durationOfBreak: TimeSpan.FromMilliseconds(1),
				                                    samplingDuration: TimeSpan.FromMilliseconds(20)
			                                    );

			return new HttpClientSettings(
				_httpClientTimeout,
				_timeoutPerTry,
				_retrySettings ?? JitterRetrySettings.Default(),
				defaultCircuitBreakerSettings);
		}
	}
}
