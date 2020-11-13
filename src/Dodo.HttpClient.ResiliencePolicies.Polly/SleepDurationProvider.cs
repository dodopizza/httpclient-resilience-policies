using System;
using System.Collections.Generic;
using Polly.Contrib.WaitAndRetry;

namespace Dodo.HttpClientResiliencePolicies.RetryPolicy
{
	internal static class SleepDurationImplementation
	{
		public static IEnumerable<TimeSpan> Convert(ISleepDurationFunction function)
		{
			switch (function)
			{
				case ConstantFunction f:
					return Convert(f);
				case LinearFunction f:
					return Convert(f);
				case ExponentialFunction f:
					return Convert(f);
				case JitterFunction f:
					return Convert(f);
				default:
					throw new NotSupportedException($"Function type={function.GetType()} not supported!");
			}
		}

		private static IEnumerable<TimeSpan> Convert(ConstantFunction function)
		{
			return Backoff.ConstantBackoff(function.InitialDelay, function.RetryCount);
		}

		private static IEnumerable<TimeSpan> Convert(LinearFunction function)
		{
			return Backoff.LinearBackoff(function.InitialDelay, function.RetryCount);
		}

		private static IEnumerable<TimeSpan> Convert(ExponentialFunction function)
		{
			return Backoff.ExponentialBackoff(function.InitialDelay, function.RetryCount);
		}

		private static IEnumerable<TimeSpan> Convert(JitterFunction function)
		{
			return Backoff.DecorrelatedJitterBackoffV2(function.MedianFirstRetryDelay, function.RetryCount);
		}
	}

}
