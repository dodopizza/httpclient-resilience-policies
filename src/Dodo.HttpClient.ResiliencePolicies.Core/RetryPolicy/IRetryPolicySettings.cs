using System;
using Dodo.HttpClientResiliencePolicies.RetryPolicy;

namespace Dodo.HttpClientResiliencePolicies.Core.RetryPolicy
{
	public interface IRetryPolicySettings
	{
		ISleepDurationFunction SleepDurationFunction { get; }
		Action<PolicyResult, TimeSpan> OnRetry { get; set; }
	}
}
