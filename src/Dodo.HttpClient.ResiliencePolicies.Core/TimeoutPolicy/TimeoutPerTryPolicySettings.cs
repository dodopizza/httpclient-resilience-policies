using System;

namespace Dodo.HttpClientResiliencePolicies.Core.TimeoutPolicy
{
	public sealed class TimeoutPerTryPolicySettings : ITimeoutPolicySettings
	{
		public TimeSpan Timeout { get; }

		public TimeoutPerTryPolicySettings(TimeSpan timeout)
		{
			Timeout = timeout;
		}

		public static ITimeoutPolicySettings Default()
		{
			return new TimeoutPerTryPolicySettings(TimeSpan.FromMilliseconds(Defaults.Timeout.TimeoutPerTryInMilliseconds));
		}
	}
}
