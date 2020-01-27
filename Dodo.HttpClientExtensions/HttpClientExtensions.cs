using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Dodo.HttpClientExtensions
{
    public static class HttpClientExtensions
    {
        public static async Task<T> Get<T>(this HttpClient httpClient, string url, CancellationToken ct)
        {
            var response = await httpClient.GetAsync(url, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await GetResponse<T>(response).ConfigureAwait(false);
        }

        public static async Task<T> GetResponse<T>(this HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<T>(responseContent);
        }

        public static Task<HttpResponseMessage> Post<TRequest>(this HttpClient httpClient, string url, TRequest request, CancellationToken ct)
        {
            return httpClient.PostAsync(new Uri(url, UriKind.Relative), GetContent(request), ct);
        }

        public static Task<HttpResponseMessage> Put<TRequest>(this HttpClient httpClient, string url, TRequest request, CancellationToken ct)
        {
            return httpClient.PutAsync(new Uri(url, UriKind.Relative), GetContent(request), ct);
        }
		
        public static Task<HttpResponseMessage> Delete(this HttpClient httpClient, string url, CancellationToken ct)
        {
            return httpClient.DeleteAsync(url, ct);
        }

        private static HttpContent GetContent<T>(T request)
        {
            var json = JsonConvert.SerializeObject(request);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            return stringContent;
        }

        public static Task<T> Get<T>(this HttpClient httpClient, string url, IEnumerable<KeyValuePair<string,string>> data, CancellationToken ct)
        {
            return httpClient.Get<T>(GetUrl(url, data), ct);
        }
		
        private static string GetUrl(string baseUrl, IEnumerable<KeyValuePair<string,string>> parameters)
        {
            return $"{baseUrl}?{string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"))}";
        }
    }
}