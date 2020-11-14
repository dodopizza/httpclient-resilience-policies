using System;

namespace Dodo.HttpClientResiliencePolicies.Core.TimeoutPolicy
{
	public sealed class OverallTimeoutPolicySettings : ITimeoutPolicySettings
	{
		public TimeSpan Timeout { get; }

		public OverallTimeoutPolicySettings(TimeSpan timeout)
		{
			Timeout = timeout;
		}

		public static ITimeoutPolicySettings Default()
		{
			return new OverallTimeoutPolicySettings(TimeSpan.FromMilliseconds(Defaults.Timeout.TimeoutOverallInMilliseconds));
		}
	}
}
