using System.Net.Http;
using Dodo.HttpClientResiliencePolicies.Core.TimeoutPolicy;
using Polly;
using Polly.Timeout;

namespace Dodo.HttpClientResiliencePolicies.Polly.PoliciesBuilders
{
	internal sealed class TimeoutPolicyBuilder
	{
		public AsyncTimeoutPolicy<HttpResponseMessage> Build(ITimeoutPolicySettings settings)
		{
			return Policy.TimeoutAsync<HttpResponseMessage>(settings.Timeout);
		}
	}
}
