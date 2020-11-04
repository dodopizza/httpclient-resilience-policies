using System;
using System.Collections.Generic;
using System.Text;

namespace Dodo.HttpClientResiliencePolicies.RetryPolicy
{
	public static class RetryAfterExtension
	{
		public static IRetryPolicySettings WithRetryAfter(this RetryPolicySettings retryPolicySettings)
		{
			retryPolicySettings.EnableRetryAfterFeature();
			return retryPolicySettings;
		}
	}
}
