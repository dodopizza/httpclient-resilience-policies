using System.Net.Http;
using Dodo.HttpClientResiliencePolicies.Tests.Fakes;

namespace Dodo.HttpClientResiliencePolicies.Tests.DSL
{
	public class HttpClientWrapper
	{
		private readonly System.Net.Http.HttpClient _client;
		private readonly MockHttpMessageHandler _handler;

		public System.Net.Http.HttpClient Client => _client;
		public long NumberOfCalls => _handler.NumberOfCalls;

		public HttpClientWrapper(System.Net.Http.HttpClient client, MockHttpMessageHandler handler)
		{
			_client = client;
			_handler = handler;
		}
	}
}
