using System;

namespace Dodo.HttpClientResiliencePolicies.Core.TimeoutPolicy
{
	public interface ITimeoutPolicySettings
	{
		TimeSpan Timeout { get; }
	}
}
