using System;

namespace Dodo.HttpClientResiliencePolicies.Core.TimeoutPolicy
{
	public interface ITimeoutPolicySettings : IPolicySettings
	{
		TimeSpan Timeout { get; }
	}
}
