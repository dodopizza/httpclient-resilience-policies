using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Dodo.HttpClientResiliencePolicies.Tests.Fakes
{
	public class MockHttpMessageHandler : HttpMessageHandler
	{
		private readonly Dictionary<string, HttpStatusCode> _hostsResponseCodes;
		private readonly TimeSpan _latency;

		public long NumberOfCalls => _numberOfCalls;
		private long _numberOfCalls = 0;

		private DateTime? _retryAfterDate;
		private TimeSpan? _retryAfterSpan;

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

		public void SetRetryAfterResponseHeader(DateTime retryAfterDate)
		{
			_retryAfterDate = retryAfterDate;
		}

		public void SetRetryAfterResponseHeader(TimeSpan retryAfterSpan)
		{
			_retryAfterSpan = retryAfterSpan;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
			CancellationToken cancellationToken)
		{
			Interlocked.Increment(ref _numberOfCalls);

			await Task.Delay(_latency, cancellationToken);

			var statusCode = _hostsResponseCodes.ContainsKey(request.RequestUri.Host)
				? _hostsResponseCodes[request.RequestUri.Host]
				: _hostsResponseCodes[string.Empty];

			var result = new HttpResponseMessage
			{
				RequestMessage = request,
				StatusCode = statusCode
			};

			if (_retryAfterDate.HasValue)
			{
				result.Headers.RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(_retryAfterDate.Value);
			}

			if (_retryAfterSpan.HasValue)
			{
				result.Headers.RetryAfter
					= new System.Net.Http.Headers.RetryConditionHeaderValue(_retryAfterSpan.Value);
			}

			return await Task.FromResult(result);
		}
	}
}
