using System;
using System.Collections.Generic;
using System.Text;

namespace Dodo.HttpClientResiliencePolicies.TimeoutPolicySettings
{
	public interface ITimeoutPolicySettings
	{
		TimeSpan Timeout { get; set; }
	}
}
