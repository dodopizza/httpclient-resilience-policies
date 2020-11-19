using System;

namespace Dodo.HttpClientResiliencePolicies.TimeoutPolicy
{
	public class TimeoutPolicySettings
	{
		public TimeSpan Timeout { get; }

		public TimeoutPolicySettings()
		{
			Timeout = TimeSpan.FromMilliseconds(Defaults.Timeout.TimeoutOverallInMilliseconds);
		}

		public TimeoutPolicySettings(TimeSpan timeout)
		{
			Timeout = timeout;
		}
	}
}
