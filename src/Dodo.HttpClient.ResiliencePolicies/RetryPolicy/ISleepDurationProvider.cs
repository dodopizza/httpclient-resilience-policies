using System;
using System.Collections.Generic;

namespace Dodo.HttpClientResiliencePolicies.RetryPolicy
{
	public interface ISleepDurationProvider
	{
		int RetryCount { get; }
		IEnumerable<TimeSpan> Durations { get; }
	}
}
