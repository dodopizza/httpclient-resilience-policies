namespace Dodo.HttpClientResiliencePolicies.Core.RetryPolicy.SleepDurationFunctions
{
	public interface ISleepDurationFunction
	{
		int RetryCount { get; }
	}
}
