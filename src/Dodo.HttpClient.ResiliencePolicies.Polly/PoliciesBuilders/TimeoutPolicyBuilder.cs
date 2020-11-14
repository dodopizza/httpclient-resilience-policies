using System.Net.Http;
using Dodo.HttpClientResiliencePolicies.Core.TimeoutPolicy;
using Polly;

namespace Dodo.HttpClientResiliencePolicies.Polly.PoliciesBuilders
{
	internal sealed class TimeoutPolicyBuilder : IPolicyBuilder
	{
		private readonly ITimeoutPolicySettings _settings;

		public TimeoutPolicyBuilder(ITimeoutPolicySettings settings)
		{
			_settings = settings;
		}

		public IAsyncPolicy<HttpResponseMessage> Build()
		{
			return Policy.TimeoutAsync<HttpResponseMessage>(_settings.Timeout);
		}
	}
}
