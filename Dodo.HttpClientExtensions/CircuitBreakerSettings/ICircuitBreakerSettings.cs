using System;
using System.Net.Http;
using Polly;

namespace Dodo.HttpClientExtensions
{
    public interface ICircuitBreakerSettings
    {
        double FailureThreshold { get; }
        int MinimumThroughput { get; }
        TimeSpan DurationOfBreak { get; }
        TimeSpan SamplingDuration { get; }
        Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnBreak { get; }
        Action OnReset { get; }
        Action OnHalfOpen { get; }
    }
}
