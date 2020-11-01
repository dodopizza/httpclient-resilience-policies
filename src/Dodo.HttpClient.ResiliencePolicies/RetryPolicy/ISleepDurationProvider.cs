using System;
using System.Collections.Generic;

namespace Dodo.HttpClientResiliencePolicies.RetryPolicy
{
	public interface ISleepDurationProvider
	{
		IEnumerable<TimeSpan> SleepFunction { get; }
	}
}
