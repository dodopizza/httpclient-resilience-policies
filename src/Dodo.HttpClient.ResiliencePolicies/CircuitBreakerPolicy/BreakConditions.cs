using System;
using System.Net;
using System.Net.Http;

namespace Dodo.HttpClientResiliencePolicies.CircuitBreakerPolicy
{
	public static class BreakConditions
	{
		public static readonly Func<HttpResponseMessage, bool> OnTooManyRequests = response =>
			response.StatusCode == (HttpStatusCode)429; // Too Many Requests
		public static readonly Func<HttpResponseMessage, bool> None = _ => false;
	}
}
