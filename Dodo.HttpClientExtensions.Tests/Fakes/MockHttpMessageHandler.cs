using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Dodo.HttpClientExtensions.Tests
{
	public class MockHttpMessageHandler : HttpMessageHandler
	{
		private readonly HttpStatusCode _statusCode;
		public long NumberOfCalls => _numberOfCalls;
		private long _numberOfCalls = 0;

		public MockHttpMessageHandler(HttpStatusCode statusCode)
		{
			_statusCode = statusCode;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
			CancellationToken cancellationToken)
		{
			Interlocked.Increment(ref _numberOfCalls);
			return await Task.FromResult(
				new HttpResponseMessage
				{
					RequestMessage = request,
					StatusCode = _statusCode
				});
		}
	}
}
