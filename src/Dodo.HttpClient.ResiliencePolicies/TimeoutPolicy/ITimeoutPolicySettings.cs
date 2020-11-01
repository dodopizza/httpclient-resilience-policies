using System;

namespace Dodo.HttpClientResiliencePolicies.TimeoutPolicy
{
	public interface ITimeoutPolicySettings
	{
		TimeSpan Timeout { get; }
	}
}
