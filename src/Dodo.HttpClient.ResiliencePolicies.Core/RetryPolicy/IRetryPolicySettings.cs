using System;
using Dodo.HttpClientResiliencePolicies.Core;

namespace Dodo.HttpClientResiliencePolicies.RetryPolicy
{
	public interface IRetryPolicySettings
	{
		ISleepDurationFunction SleepDurationFunction { get; }
		Action<PolicyResult, TimeSpan> OnRetry { get; set; }
	}
}
