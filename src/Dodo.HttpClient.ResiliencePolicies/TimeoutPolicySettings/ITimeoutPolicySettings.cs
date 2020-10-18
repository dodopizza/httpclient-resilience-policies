using System;

namespace Dodo.HttpClientResiliencePolicies.TimeoutPolicySettings
{
	public interface ITimeoutPolicySettings
	{
		TimeSpan Timeout { get; set; }
	}
}
