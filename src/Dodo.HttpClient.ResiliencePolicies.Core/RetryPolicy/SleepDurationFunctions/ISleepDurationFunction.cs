namespace Dodo.HttpClientResiliencePolicies.RetryPolicy
{
	public interface ISleepDurationFunction
	{
		int RetryCount { get; }
	}
}
