using System;
using System.Collections.Generic;

namespace Dodo.HttpClientResiliencePolicies.RetrySettings
{
	public interface ISleepDurationProvider
	{
		IEnumerable<TimeSpan> SleepFunction { get; }
	}
}
