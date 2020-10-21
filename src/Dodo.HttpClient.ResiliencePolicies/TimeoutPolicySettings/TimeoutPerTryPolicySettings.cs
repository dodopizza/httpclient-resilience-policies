using System;

namespace Dodo.HttpClientResiliencePolicies.TimeoutPolicySettings
{
	public sealed class TimeoutPerTryPolicySettings : ITimeoutPolicySettings
	{
		public TimeoutPerTryPolicySettings()
		{
			Timeout = TimeSpan.FromMilliseconds(Defaults.Timeout.TimeoutPerTryInMilliseconds);
		}

		public TimeSpan Timeout { get; set; }
	}
}
