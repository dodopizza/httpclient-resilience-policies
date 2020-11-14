using System;
using Dodo.HttpClientResiliencePolicies.Core.RetryPolicy.SleepDurationFunctions;

namespace Dodo.HttpClientResiliencePolicies.Core.RetryPolicy
{
	public interface IRetryPolicySettings
	{
		ISleepDurationFunction SleepDurationFunction { get; }
		Action<PolicyResult, TimeSpan> OnRetry { get; set; }
	}
}
