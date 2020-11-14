using System;

namespace Dodo.HttpClientResiliencePolicies.Core.TimeoutPolicy
{
	public sealed class TimeoutPerTryPolicySettings : ITimeoutPolicySettings
	{
		public TimeSpan Timeout { get; }

		public TimeoutPerTryPolicySettings()
		{
			Timeout = TimeSpan.FromMilliseconds(Defaults.Timeout.TimeoutPerTryInMilliseconds);
		}

		public TimeoutPerTryPolicySettings(TimeSpan timeout)
		{
			Timeout = timeout;
		}
	}
}
