using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Dodo.HttpClient.ResiliencePolicies.Tests.Fakes
{
	public class MockHttpMessageHandler : HttpMessageHandler
	{
		private readonly Dictionary<string, HttpStatusCode> _hostsResponseCodes;
		private readonly TimeSpan _latency;

		public long NumberOfCalls => _numberOfCalls;
		private long _numberOfCalls = 0;

		public MockHttpMessageHandler(HttpStatusCode statusCode, TimeSpan latency)
			: this(new Dictionary<string, HttpStatusCode> {{string.Empty, statusCode}}, latency)
		{
		}

		public MockHttpMessageHandler(Dictionary<string, HttpStatusCode> hostsResponseCodes, TimeSpan latency)
		{
			if (hostsResponseCodes != null && hostsResponseCodes.Count > 0)
			{
				_hostsResponseCodes = hostsResponseCodes;
			}
			else
			{
				_hostsResponseCodes = new Dictionary<string, HttpStatusCode> {{string.Empty, HttpStatusCode.OK}};
			}

			_latency = latency;
		}


		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
			CancellationToken cancellationToken)
		{
			Interlocked.Increment(ref _numberOfCalls);

			await Task.Delay(_latency, cancellationToken);

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
