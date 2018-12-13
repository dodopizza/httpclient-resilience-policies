using System;

namespace Dodo.HttpClientExtensions
{
	public class ClientSettings
	{
		public TimeSpan TotalTimeOut { get; }
		public TimeSpan TimeOutPerRequest { get; }

		public int RetryCount { get; }
		public Func<int, TimeSpan> SleepDurationProvider { get; }

		public double FailureThreshold { get; }
		public int MinimumThroughput { get; }
		public TimeSpan DurationOfBreak { get; }
		public TimeSpan SamplingDuration { get; }

		public ClientSettings(
			TimeSpan totalTimeOut, 
			TimeSpan timeOutPerRequest, 
			int retryCount,
			Func<int, TimeSpan> sleepDurationProvider, 
			double failureThreshold, 
			int minimumThroughput,
			TimeSpan durationOfBreak, 
			TimeSpan samplingDuration
			)
		{
			TotalTimeOut = totalTimeOut;
			TimeOutPerRequest = timeOutPerRequest;
			RetryCount = retryCount;
			SleepDurationProvider = sleepDurationProvider;
			FailureThreshold = failureThreshold;
			MinimumThroughput = minimumThroughput;
			DurationOfBreak = durationOfBreak;
			SamplingDuration = samplingDuration;
		}
		
		public ClientSettings(
			TimeSpan totalTimeOut, 
			TimeSpan timeOutPerRequest, 
			int retryCount
			)
		{
			TotalTimeOut = totalTimeOut;
			TimeOutPerRequest = timeOutPerRequest;
			RetryCount = retryCount;
			SleepDurationProvider = _defaultSleepDurationProvider;
			FailureThreshold = DefaultFailureThreshold;
			MinimumThroughput = DefaultMinimumThroughput;
			DurationOfBreak = TimeSpan.FromSeconds(DefaultDurationOfBreakInSec);
			SamplingDuration = TimeSpan.FromSeconds(DefaultSamplingDurationInSec);
		}

		public static ClientSettings Default() =>
			new ClientSettings(
				TimeSpan.FromMilliseconds(DefaultTotalTimeOutInMilliseconds),
				TimeSpan.FromMilliseconds(DefaultTimeOutPerRequestInMilliseconds),
				DefaultRetryCount,
				_defaultSleepDurationProvider,
				DefaultFailureThreshold,
				DefaultMinimumThroughput,
				TimeSpan.FromSeconds(DefaultDurationOfBreakInSec),
				TimeSpan.FromSeconds(DefaultSamplingDurationInSec)
			);
		private const int DefaultTotalTimeOutInMilliseconds = 10000;
		private const int DefaultTimeOutPerRequestInMilliseconds = 2000;
		private const int DefaultRetryCount = 5;
		private static readonly Func<int, TimeSpan> _defaultSleepDurationProvider = i => TimeSpan.FromMilliseconds(20 * Math.Pow(2, i));
		private const double DefaultFailureThreshold = 0.5;
		private const int DefaultMinimumThroughput = 10;
		private const int DefaultDurationOfBreakInSec = 5;
		private const int DefaultSamplingDurationInSec = 30;
	}
}