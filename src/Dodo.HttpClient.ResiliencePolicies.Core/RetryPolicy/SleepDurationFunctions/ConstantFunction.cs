using System;

namespace Dodo.HttpClientResiliencePolicies.Core.RetryPolicy.SleepDurationFunctions
{
	public class ConstantFunction : ISleepDurationFunction
	{
		public int RetryCount { get; }
		public TimeSpan InitialDelay { get; }

		public ConstantFunction(int retryCount, TimeSpan initialDelay)
		{
			if (retryCount < 0) throw new ArgumentOutOfRangeException(nameof(retryCount), retryCount, "should be >= 0");
			if (initialDelay < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(initialDelay), initialDelay, "should be >= 0ms");

			RetryCount = retryCount;
			InitialDelay = initialDelay;
		}
	}
}
