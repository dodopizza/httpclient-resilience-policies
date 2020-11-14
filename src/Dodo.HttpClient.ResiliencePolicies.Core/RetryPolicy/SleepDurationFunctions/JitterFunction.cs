using System;

namespace Dodo.HttpClientResiliencePolicies.Core.RetryPolicy.SleepDurationFunctions
{
	public class JitterFunction : ISleepDurationFunction
	{
		public int RetryCount { get; }
		public TimeSpan MedianFirstRetryDelay { get; }

		public JitterFunction(int retryCount, TimeSpan medianFirstRetryDelay)
		{
			if (retryCount < 0) throw new ArgumentOutOfRangeException(nameof(retryCount), retryCount, "should be >= 0");
			if (medianFirstRetryDelay < TimeSpan.Zero)
				throw new ArgumentOutOfRangeException(nameof(medianFirstRetryDelay), medianFirstRetryDelay,
					"should be >= 0ms");

			RetryCount = retryCount;
			MedianFirstRetryDelay = medianFirstRetryDelay;
		}
	}
}
