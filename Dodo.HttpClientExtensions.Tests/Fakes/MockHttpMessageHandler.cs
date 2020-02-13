using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Dodo.HttpClientExtensions.Tests
{
	public class MockHttpMessageHandler : HttpMessageHandler
	{
		private readonly Dictionary<string, HttpStatusCode> _hostsResponseCodes;

		public long NumberOfCalls => _numberOfCalls;
		private long _numberOfCalls = 0;

		public MockHttpMessageHandler(HttpStatusCode statusCode)
		{
			_hostsResponseCodes = new Dictionary<string, HttpStatusCode> {{string.Empty, statusCode}};
		}

		public MockHttpMessageHandler(Dictionary<string, HttpStatusCode> hostsResponseCodes)
		{
			if (hostsResponseCodes != null && hostsResponseCodes.Count > 0)
			{
				_hostsResponseCodes = hostsResponseCodes;
			}
			else
			{
				_hostsResponseCodes = new Dictionary<string, HttpStatusCode> {{string.Empty, HttpStatusCode.OK}};
			}
		}


		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
			CancellationToken cancellationToken)
		{
			Interlocked.Increment(ref _numberOfCalls);

			var statusCode = _hostsResponseCodes.ContainsKey(request.RequestUri.Host)
				? _hostsResponseCodes[request.RequestUri.Host]
				: _hostsResponseCodes[string.Empty];

			return await Task.FromResult(
				new HttpResponseMessage
				{
					RequestMessage = request,
					StatusCode = statusCode
				});
		}
	}
}
