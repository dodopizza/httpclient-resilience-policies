using System;

namespace Dodo.HttpClientResiliencePolicies.TimeoutPolicy
{
	public interface ITimeoutPolicySettings : IPolicySettings
	{
		TimeSpan Timeout { get; }
	}
}
