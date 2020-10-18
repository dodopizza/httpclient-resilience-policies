using System;

namespace Dodo.HttpClientResiliencePolicies.TimeoutPolicySettings
{
	public sealed class OverallTimeoutPolicySettings : ITimeoutPolicySettings
	{
		public TimeSpan Timeout { get; set; }

		public static ITimeoutPolicySettings Default() => new OverallTimeoutPolicySettings
		{
			Timeout = TimeSpan.FromMilliseconds(Defaults.Timeout.TimeoutOverallInMilliseconds)
		};
	}
}
