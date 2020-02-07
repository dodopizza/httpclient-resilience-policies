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
            ClientSettings settings,
            string clientName = null) where TClientInterface : class
            where TClientImplementation : class, TClientInterface
        {
            var httpClientBuilder = sc.AddHttpClient<TClientInterface, TClientImplementation>(client =>
                {
                    client.BaseAddress = baseAddress;
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.Timeout = settings.TimeoutPerRequest;
                })
                .AddDefaultPolicies(settings);

            return httpClientBuilder;
        }

        public static IHttpClientBuilder AddDefaultPolicies(
            this IHttpClientBuilder clientBuilder,
            ClientSettings settings)
        {
            return clientBuilder
                .AddTimeoutPolicy(settings.TotalTimeout)
                .AddRetryPolicy(settings.RetrySettings)
                .AddCircuitBreakerPolicy(settings.CircuitBreakerSettings);
        }

        private static IHttpClientBuilder AddRetryPolicy(
            this IHttpClientBuilder clientBuilder,
            RetrySettings settings)
        {
            return clientBuilder
                .AddPolicyHandler(HttpPolicyExtensions // (serviceProvider, _)  =>
                    .HandleTransientHttpError()
                    .Or<TimeoutRejectedException>()
                    .WaitAndRetryAsync(
                        settings.RetryCount,
                        settings.SleepDurationProvider,
                        settings.OnRetry));
        }

        private static IHttpClientBuilder AddCircuitBreakerPolicy(
            this IHttpClientBuilder clientBuilder,
            CircuitBreakerSettings settings)
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
