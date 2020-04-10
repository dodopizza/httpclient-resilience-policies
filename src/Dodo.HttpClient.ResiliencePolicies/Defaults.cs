namespace Dodo.HttpClient.ResiliencePolicies
{
	public static class Defaults
	{
		public static class Timeout
		{
			public const int HttpClientTimeoutInMilliseconds = 10000;
			public const int TimeoutPerTryInMilliseconds = 2000;
		}

		public static class Retry
		{
			public const int RetryCount = 2;
			public const int MedianFirstRetryDelayInMilliseconds = 2000;
		}

		public static class CircuitBreaker
		{
			public const double FailureThreshold = 0.5;
			public const int MinimumThroughput = 10;
			public const int DurationOfBreakInMilliseconds = 5000;
			public const int SamplingDurationInMilliseconds = 30000;
		}
	}
}
