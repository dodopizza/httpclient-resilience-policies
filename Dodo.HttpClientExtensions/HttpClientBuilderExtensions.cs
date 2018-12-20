using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using zipkin4net.Transport.Http;

namespace Dodo.HttpClientExtensions
{
	public static class HttpClientBuilderExtensions
	{
		public static IHttpClientBuilder AddJsonClient<TClientInterface, TClientImplementation>(
			this IServiceCollection sc,
			Uri baseAddress,
			ClientSettings settings,
			string clientName = null) where TClientInterface : class where TClientImplementation : class, TClientInterface
		{
			var httpClientBuilder = sc.AddHttpClient<TClientInterface, TClientImplementation>(client =>
				{
					client.BaseAddress = baseAddress;
					client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
					client.Timeout = settings.TimeOutPerRequest;
				})
				.AddDefaultPolicies(settings);
			if (!string.IsNullOrEmpty(clientName))
			{
				httpClientBuilder.AddTracingHandler(clientName);
			}
			return httpClientBuilder;
		}

		public static IHttpClientBuilder AddDefaultPolicies(this IHttpClientBuilder clientBuilder, ClientSettings settings)
		{
			return clientBuilder
				.AddTimeoutPolicy(settings.TotalTimeOut)
				.AddRetryPolicy(settings.RetryCount, settings.SleepDurationProvider)
				.AddCircuitBreakerPolicy(settings.FailureThreshold, settings.MinimumThroughput, settings.DurationOfBreak, settings.SamplingDuration);
		}

		public static IHttpClientBuilder AddTracingHandler(this IHttpClientBuilder clientBuilder, string clientName)
		{
			return clientBuilder
				.AddHttpMessageHandler(() => TracingHandler.WithoutInnerHandler(clientName));
		}

		private static IHttpClientBuilder AddRetryPolicy(this IHttpClientBuilder clientBuilder, int retryCount, Func<int, TimeSpan> sleepDurationProvider)
		{
			return clientBuilder
				.AddPolicyHandler(HttpPolicyExtensions
					.HandleTransientHttpError()
					.Or<TimeoutRejectedException>()
					.WaitAndRetryAsync(retryCount, sleepDurationProvider));
		}

		private static IHttpClientBuilder AddCircuitBreakerPolicy(
			this IHttpClientBuilder clientBuilder,
			double failureThreshold,
			int minimumThroughput,
			TimeSpan durationOfBreak,
			TimeSpan samplingDuration)
		{
			return clientBuilder.AddPolicyHandler( 
				HttpPolicyExtensions
					.HandleTransientHttpError()
					.AdvancedCircuitBreakerAsync(
						failureThreshold: failureThreshold,
						samplingDuration: samplingDuration, 
						minimumThroughput: minimumThroughput,
						durationOfBreak: durationOfBreak));
		}

		private static IHttpClientBuilder AddTimeoutPolicy(this IHttpClientBuilder httpClientBuilder, TimeSpan timeout)
		{
			return httpClientBuilder.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(timeout));
		}
	}
}