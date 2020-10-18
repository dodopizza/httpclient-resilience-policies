using System;

namespace Dodo.HttpClientResiliencePolicies.TimeoutPolicySettings
{
	public sealed class TimeoutPerTryPolicySettings : ITimeoutPolicySettings
	{
		public TimeSpan Timeout { get; set; }

		public static ITimeoutPolicySettings Default() => new TimeoutPerTryPolicySettings
		{
			Timeout = TimeSpan.FromMilliseconds(Defaults.Timeout.TimeoutPerTryInMilliseconds)
		};
	}
}
