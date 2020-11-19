using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Dodo.HttpClientResiliencePolicies.CircuitBreakerPolicy;
using Dodo.HttpClientResiliencePolicies.RetryPolicy;
using Dodo.HttpClientResiliencePolicies.Tests.Fakes;
using Dodo.HttpClientResiliencePolicies.TimeoutPolicy;
using Microsoft.Extensions.DependencyInjection;

namespace Dodo.HttpClientResiliencePolicies.Tests.DSL
{
	public sealed class HttpClientWrapperBuilder
	{
		private const string ClientName = "TestClient";
		private readonly Uri _uri = new Uri("http://localhost");

		private readonly Dictionary<string, HttpStatusCode> _hostsResponseCodes =
			new Dictionary<string, HttpStatusCode>();

		private ResiliencePoliciesSettings _fullResiliencePoliciesSettings;
		private RetryPolicySettings _retryPolicySettings;
		private CircuitBreakerPolicySettings _circuitBreakerPolicySettings;
		private TimeSpan _timeoutPerTry = TimeSpan.FromDays(1);
		private TimeSpan _timeoutOverall = TimeSpan.FromDays(1);
		private TimeSpan _responseLatency = TimeSpan.Zero;
		private TimeSpan? _retryAfterSpan;
		private DateTime? _retryAfterDate;

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

		public HttpClientWrapperBuilder WithTimeoutOverall(TimeSpan timeoutOverall)
		{
			_timeoutOverall = timeoutOverall;
			return this;
		}

		public HttpClientWrapperBuilder WithTimeoutPerTry(TimeSpan timeoutPerTry)
		{
			_timeoutPerTry = timeoutPerTry;
			return this;
		}

		public HttpClientWrapperBuilder WithRetrySettings(RetryPolicySettings retrySettings)
		{
			_retryPolicySettings = retrySettings;
			return this;
		}

		public HttpClientWrapperBuilder WithCircuitBreakerSettings(CircuitBreakerPolicySettings circuitBreakerSettings)
		{
			_circuitBreakerPolicySettings = circuitBreakerSettings;
			return this;
		}

		public HttpClientWrapperBuilder WithFullResiliencePolicySettings(ResiliencePoliciesSettings resiliencePoliciesSettings)
		{
			_fullResiliencePoliciesSettings = resiliencePoliciesSettings;
			return this;
		}

		public HttpClientWrapperBuilder WithResponseLatency(TimeSpan responseLatency)
		{
			_responseLatency = responseLatency;
			return this;
		}

		public HttpClientWrapperBuilder WithRetryAfterHeader(TimeSpan delay)
		{
			_retryAfterSpan = delay;
			return this;
		}

		public HttpClientWrapperBuilder WithRetryAfterHeader(DateTime date)
		{
			_retryAfterDate = date;
			return this;
		}

		public HttpClientWrapper Please()
		{
			var handler = new MockHttpMessageHandler(_hostsResponseCodes, _responseLatency);

			if (_retryAfterDate.HasValue)
			{
				handler.SetRetryAfterResponseHeader(_retryAfterDate.Value);
			}

			if (_retryAfterSpan.HasValue)
			{
				handler.SetRetryAfterResponseHeader(_retryAfterSpan.Value);
			}

			var settings = _fullResiliencePoliciesSettings ?? BuildClientSettings();
			var services = new ServiceCollection();
			services
				.AddJsonClient<IMockJsonClient, MockJsonClient>(_uri, settings, ClientName)
				.ConfigurePrimaryHttpMessageHandler(() => handler);

			var serviceProvider = services.BuildServiceProvider();
			var factory = serviceProvider.GetService<IHttpClientFactory>();
			var client = factory?.CreateClient(ClientName) ??
			             throw new NullReferenceException($"\"{nameof(factory)}\" was not created properly");
			return new HttpClientWrapper(client, handler);
		}

		private ResiliencePoliciesSettings BuildClientSettings()
		{
			var defaultCircuitBreakerSettings = _circuitBreakerPolicySettings ?? new CircuitBreakerPolicySettings
			(
				failureThreshold: 0.5,
				minimumThroughput: int.MaxValue,
				durationOfBreak: TimeSpan.FromMilliseconds(1),
				samplingDuration: TimeSpan.FromMilliseconds(20)
			);

			return new ResiliencePoliciesSettings
			{
				OverallTimeoutPolicySettings = new TimeoutPolicySettings(_timeoutOverall),
				TimeoutPerTryPolicySettings = new TimeoutPolicySettings(_timeoutPerTry),
				RetryPolicySettings = _retryPolicySettings ?? new RetryPolicySettings(),
				CircuitBreakerPolicySettings = defaultCircuitBreakerSettings
			};
		}
	}
}
