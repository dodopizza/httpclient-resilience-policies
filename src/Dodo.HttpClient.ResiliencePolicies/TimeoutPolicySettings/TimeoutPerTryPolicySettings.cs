using System;
using System.Collections.Generic;
using System.Text;

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
