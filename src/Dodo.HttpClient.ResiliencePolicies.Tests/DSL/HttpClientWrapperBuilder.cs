using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Dodo.HttpClient.ResiliencePolicies.CircuitBreakerSettings;
using Dodo.HttpClient.ResiliencePolicies.FallbackSettings;
using Dodo.HttpClient.ResiliencePolicies.RetrySettings;
using Dodo.HttpClient.ResiliencePolicies.Tests.Fakes;
using Microsoft.Extensions.DependencyInjection;

namespace Dodo.HttpClient.ResiliencePolicies.Tests.DSL
{
	public sealed class HttpClientWrapperBuilder
	{
		private const string ClientName = "TestClient";
		private readonly Dictionary<string, HttpStatusCode> _hostsResponseCodes = new Dictionary<string, HttpStatusCode>();
		private IFallbackSettings _fallbackSettings;
		private IRetrySettings _retrySettings;
		private ICircuitBreakerSettings _circuitBreakerSettings;
		private TimeSpan _httpClientTimeout = TimeSpan.FromDays(1);
		private TimeSpan _timeoutPerTry = TimeSpan.FromDays(1);
		private TimeSpan _responseLatency = TimeSpan.Zero;

		public HttpClientWrapperBuilder WithStatusCode(HttpStatusCode statusCode)
		{
			_hostsResponseCodes.Add(string.Empty, statusCode);
			return this;
		}

		public HttpClientWrapperBuilder WithHostAndStatusCode(string host, HttpStatusCode statusCode)
		{
			_hostsResponseCodes.Add(host, statusCode);
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

		public HttpClientWrapperBuilder WithFallbackSettings(IFallbackSettings fallbackSettings)
		{
			_fallbackSettings = fallbackSettings;
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

		public HttpClientWrapperBuilder WithResponseLatency(TimeSpan responseLatency)
		{
			_responseLatency = responseLatency;
			return this;
		}

		public HttpClientWrapper Please()
		{
			var handler = new MockHttpMessageHandler(_hostsResponseCodes, _responseLatency);
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

		public HttpClientWrapper PleaseHostSpecific()
		{
			var handler = new MockHttpMessageHandler(_hostsResponseCodes, _responseLatency);
			var services = new ServiceCollection();
			services
				.AddHttpClient(ClientName, c => { c.Timeout = _httpClientTimeout; })
				.AddDefaultHostSpecificPolicies(BuildClientSettings())
				.ConfigurePrimaryHttpMessageHandler(() => handler);

			var serviceProvider = services.BuildServiceProvider();
			var factory = serviceProvider.GetService<IHttpClientFactory>();
			var client = factory.CreateClient(ClientName);
			return new HttpClientWrapper(client, handler);
		}

		private HttpClientSettings BuildClientSettings()
		{
			var defaultCircuitBreakerSettings = _circuitBreakerSettings ?? new CircuitBreakerSettings.CircuitBreakerSettings(
				failureThreshold: 0.5,
				minimumThroughput: int.MaxValue,
				durationOfBreak: TimeSpan.FromMilliseconds(1),
				samplingDuration: TimeSpan.FromMilliseconds(20)
				);

			return new HttpClientSettings(
				_httpClientTimeout,
				_timeoutPerTry,
				_retrySettings ?? JitterRetrySettings.Default(),
				defaultCircuitBreakerSettings,
				_fallbackSettings);
		}
	}
}
