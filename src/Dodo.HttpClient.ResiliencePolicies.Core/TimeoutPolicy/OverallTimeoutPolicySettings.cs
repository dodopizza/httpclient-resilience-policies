using System;

namespace Dodo.HttpClientResiliencePolicies.Core.TimeoutPolicy
{
	public sealed class OverallTimeoutPolicySettings : ITimeoutPolicySettings
	{
		public TimeSpan Timeout { get; }

		public OverallTimeoutPolicySettings()
		{
			Timeout = TimeSpan.FromMilliseconds(Defaults.Timeout.TimeoutOverallInMilliseconds);
		}

		public OverallTimeoutPolicySettings(TimeSpan timeout)
		{
			Timeout = timeout;
		}
	}
}
