using System;
using System.Collections.Generic;

namespace Dodo.HttpClientResiliencePolicies.RetryPolicy
{
	public class SleepDurationProvider : ISleepDurationProvider
	{
		public int RetryCount { get; }
		public IEnumerable<TimeSpan> Durations { get; }

		public SleepDurationProvider(int retryCount, IEnumerable<TimeSpan> durations)
		{
			RetryCount = retryCount;
			Durations = durations;
			throw new NotImplementedException();
		}
	}
}
