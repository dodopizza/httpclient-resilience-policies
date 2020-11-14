using System;

namespace Dodo.HttpClientResiliencePolicies.RetryPolicy
{
	public class LinearFunction : ISleepDurationFunction
	{
		public int RetryCount { get; }
		public TimeSpan InitialDelay { get; }

		public LinearFunction(int retryCount, TimeSpan initialDelay)
		{
			if (retryCount < 0) throw new ArgumentOutOfRangeException(nameof(retryCount), retryCount, "should be >= 0");
			if (initialDelay < TimeSpan.Zero)
				throw new ArgumentOutOfRangeException(nameof(initialDelay), initialDelay, "should be >= 0ms");

			RetryCount = retryCount;
			InitialDelay = initialDelay;
		}
	}
}