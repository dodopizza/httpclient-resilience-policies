using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;

namespace Dodo.HttpClientResiliencePolicies
{
	public static class HttpClientFactoryServiceCollectionExtensions
	{
		/// <summary>
		/// Adds the <see cref="IHttpClientFactory"/> and related services to the <see cref="IServiceCollection"/>
		/// with pre-configured JSON headers, HttpClient Timeout and resilience policies.
		/// </summary>
		/// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the client.</returns>
		public static IHttpClientBuilder AddJsonClient<TClientInterface, TClientImplementation>(
			this IServiceCollection sc,
			Uri baseAddress,
			string clientName = null) where TClientInterface : class
			where TClientImplementation : class, TClientInterface
		{
			return AddJsonClient<TClientInterface, TClientImplementation>(
				sc, baseAddress, new ResiliencePoliciesSettings(), clientName);
		}

		/// <summary>
		/// Adds the <see cref="IHttpClientFactory"/> and related services to the <see cref="IServiceCollection"/>
		/// with pre-configured JSON headers and custom resilience policies.
		/// </summary>
		/// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the client.</returns>
		public static IHttpClientBuilder AddJsonClient<TClientInterface, TClientImplementation>(
			this IServiceCollection sc,
			Uri baseAddress,
			ResiliencePoliciesSettings settings,
			string clientName = null) where TClientInterface : class
			where TClientImplementation : class, TClientInterface
		{
			var delta = TimeSpan.FromMilliseconds(1000);

			void DefaultClient(System.Net.Http.HttpClient client)
			{
				client.BaseAddress = baseAddress;
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				client.Timeout = settings.OverallTimeoutPolicySettings.Timeout + delta;
			}

			var httpClientBuilder = string.IsNullOrEmpty(clientName)
				? sc.AddHttpClient<TClientInterface, TClientImplementation>(DefaultClient)
				: sc.AddHttpClient<TClientInterface, TClientImplementation>(clientName, DefaultClient);

			httpClientBuilder.AddResiliencePolicies(settings);

			return httpClientBuilder;
		}
	}
}
