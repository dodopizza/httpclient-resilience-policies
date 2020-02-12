using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace Dodo.HttpClientExtensions
{
	public static class HttpClientBuilderExtensions
	{
		public static IHttpClientBuilder AddJsonClient<TClientInterface, TClientImplementation>(
			this IServiceCollection sc,
			Uri baseAddress,
			HttpClientSettings settings,
			string clientName = null) where TClientInterface : class
			where TClientImplementation : class, TClientInterface
		{
			var httpClientBuilder = sc.AddHttpClient<TClientInterface, TClientImplementation>(client =>
				{
					client.BaseAddress = baseAddress;
					client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
					client.Timeout = settings.HttpClientTimeout;
				})
				.AddDefaultPolicies(settings);

			return httpClientBuilder;
		}

		public static IHttpClientBuilder AddDefaultPolicies(
			this IHttpClientBuilder clientBuilder)
		{
			return clientBuilder
				.AddDefaultPolicies(HttpClientSettings.Default());
		}

		public static IHttpClientBuilder AddDefaultPolicies(
			this IHttpClientBuilder clientBuilder,
			HttpClientSettings settings)
		{
			return clientBuilder
				.AddRetryPolicy(settings.RetrySettings)
				.AddTimeoutPolicy(settings.TimeoutPerTry)
				.AddCircuitBreakerPolicy(settings.CircuitBreakerSettings);
		}

		private static IHttpClientBuilder AddRetryPolicy(
			this IHttpClientBuilder clientBuilder,
			IRetrySettings settings)
		{
			return clientBuilder
				.AddPolicyHandler(HttpPolicyExtensions
					.HandleTransientHttpError()
					.Or<TimeoutRejectedException>()
					.WaitAndRetryAsync(
						settings.RetryCount,
						settings.SleepDurationProvider,
						settings.OnRetry));
		}

		private static IHttpClientBuilder AddCircuitBreakerPolicy(
			this IHttpClientBuilder clientBuilder,
			ICircuitBreakerSettings settings)
		{
			return clientBuilder.AddPolicyHandler(
				HttpPolicyExtensions
					.HandleTransientHttpError()
					.OrResult(r => r.StatusCode == (HttpStatusCode) 429) // Too Many Requests
					.AdvancedCircuitBreakerAsync(
						settings.FailureThreshold,
						settings.SamplingDuration,
						settings.MinimumThroughput,
						settings.DurationOfBreak,
						settings.OnBreak,
						settings.OnReset,
						settings.OnHalfOpen));
		}

		private static IHttpClientBuilder AddTimeoutPolicy(this IHttpClientBuilder httpClientBuilder, TimeSpan timeout)
		{
			return httpClientBuilder.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(timeout));
		}
	}
}
