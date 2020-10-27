using System;
using System.Collections.Generic;
using System.Text;

namespace Dodo.HttpClientResiliencePolicies.TimeoutPolicySettings
{
	public sealed class OverallTimeoutPolicySettings : ITimeoutPolicySettings
	{
		public OverallTimeoutPolicySettings()
		{
			Timeout = TimeSpan.FromMilliseconds(Defaults.Timeout.TimeoutOverallInMilliseconds);
		}

		public TimeSpan Timeout { get; set; }
	}
}
