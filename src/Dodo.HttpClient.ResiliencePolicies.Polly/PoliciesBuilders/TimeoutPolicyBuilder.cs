using System.Net.Http;
using Dodo.HttpClientResiliencePolicies.TimeoutPolicy;
using Polly;
using Polly.Timeout;

namespace Dodo.HttpClient.ResiliencePolicies.Polly.PoliciesBuilders
{
	public class TimeoutPolicyBuilder
	{
		public AsyncTimeoutPolicy<HttpResponseMessage> Build(ITimeoutPolicySettings settings)
		{
			return Policy.TimeoutAsync<HttpResponseMessage>(settings.Timeout);
		}
	}
}
