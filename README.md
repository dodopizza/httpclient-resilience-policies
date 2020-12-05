# Dodo.HttpClient.ResiliencePolicies library

[![nuget](https://img.shields.io/nuget/v/Dodo.HttpClient.ResiliencePolicies?label=NuGet)](https://www.nuget.org/packages/Dodo.HttpClient.ResiliencePolicies)
![master](https://github.com/dodopizza/httpclient-resilience-policies/workflows/master/badge.svg)

Dodo.HttpClient.ResiliencePolicies library extends [IHttpClientBuilder](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.ihttpclientbuilder) with easy to use resilience policies for the HttpClient.

In the world of microservices it is quite important to pay attention to resilience of communications between services. You have to think about things like retries, timeouts, circuit breakers, etc.
We already have a great library for this class of problems called [Polly](https://github.com/App-vNext/Polly). It is really powerful. Polly is like a Swiss knife gives you a lot of functionality, but you should know how and when to use it. It could be a complicated task.

Main goal of our library is to hide this complexity from the end-users. It uses Polly under the hood and provides some pre-defined functionality with reasonable defaults and minimal settings to setup resilience policies atop of HttpClient.
You can just plug the with single line of code and your HttpClient will become much more robust than before.


## Functionality provided by the library

Library provides few methods which returns `IHttpClientBuilder` and you may chain it with other `HttpMessageHandler`.

There are list of public methods to use:

```csharp
// Pre-defined policies with defaults settings
IHttpClientBuilder AddResiliencePolicies(this IHttpClientBuilder clientBuilder);

// Pre-defined policies with custom settings
IHttpClientBuilder AddResiliencePolicies(this IHttpClientBuilder clientBuilder, ResiliencePoliciesSettings settings)
```

`AddResiliencePolicies` wraps HttpClient with four policies:

- Overall Timeout policy – timeout for entire request, after this time we are not interested in the result anymore.
- Retry policy – defines how much and how often we will attempt to send request again on failures.
- Circuit Breaker policy – defines when we should take a break in our retries if the upstream service doesn't respond.
- Timeout Per Try policy - timeout for each try (defined in Retry policy), after this time attempt considered as failure.

Library also provides pre-configured HttpClient:

```csharp
// Pre-defined HttpClientFactory which is configured to work with `application/json` MIME media type and uses default ResiliencePolicies
IHttpClientBuilder AddJsonClient<TClientInterface, TClientImplementation>(
			this IServiceCollection sc,
			Uri baseAddress,
			string clientName = null)

// Pre-defined HttpClientFactory which is configured to work with `application/json` MIME media type and uses ResiliencePolicies with custom settings
IHttpClientBuilder AddJsonClient<TClientInterface, TClientImplementation>(
			this IServiceCollection sc,
			Uri baseAddress,
			ResiliencePoliciesSettings settings,
			string clientName = null)
```

Custom settings can be provided via `ResiliencePoliciesSettings` (see examples below).  
Also you may check the [defaults](src/Dodo.HttpClient.ResiliencePolicies/Defaults.cs) provided by the library (all of this can be overriden in custom settings).

## Usage examples

1. Using default client provided by the library and add it to the `ServiceCollection` in the Startup like this:

    ```csharp
    using Dodo.HttpClientResiliencePolicies;
    ...

    service                         // IServiceCollection
        .AddJsonClient(...)         // HttpClientFactory to build JsonClient provided by the library with all defaults
    ```

2. Add resilience policy with default settings to existing HttpClient

    ```csharp
    using Dodo.HttpClientResiliencePolicies;
    ...
   
    service                         // IServiceCollection
        .AddHttpClient(...)         // Existing HttpClientFactory
        .AddResiliencePolicies()    // Pre-defined resilience policies with all defaults
   ```

3. Define custom settings for resilience policies:

    ```csharp
    using Dodo.HttpClientResiliencePolicies;
    ...
    
    var settings = new ResiliencePoliciesSettings
    {
        OverallTimeout = TimeSpan.FromSeconds(50),
        TimeoutPerTry = TimeSpan.FromSeconds(2),
        RetryPolicySettings = RetryPolicySettings.Jitter(2, TimeSpan.FromMilliseconds(50)),
        CircuitBreakerPolicySettings = new CircuitBreakerPolicySettings(
            failureThreshold: 0.5,
            minimumThroughput: 10,
            durationOfBreak: TimeSpan.FromSeconds(5),
            samplingDuration: TimeSpan.FromSeconds(30)
        ),
        OnRetry = (response, time) => { ... },      // Handle retry event. For example you may add logging here
        OnBreak = (response, time) => { ... },      // Handle CircuitBreaker break event. For example you may add logging here
        OnReset = () => {...},                      // Handle CircuitBreaker reset event. For example you may add logging here
        OnHalfOpen = () => {...},                   // Handle CircuitBreaker reset event. For example you may add logging here
    }
    ```

    You may provide only properties which you want to customize, the defaults will be used for the rest.  
    You may choose different retry strategies. RetryPolicySettings provides static methods to choose Constant, Linear, Exponential or Jitter (exponential with jitter backoff) strategies. Jitter is used as default strategy.
    
    You may provide settings as a parameter to `.AddJsonClient(...)` or `.AddResiliencePolicies()` to override default settings.
    
## References

- Check Polly [documentation](https://github.com/App-vNext/Polly/wiki) to learn more about each policy.
- [Use IHttpClientFactory to implement resilient HTTP requests](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests).
- [Cloud design patterns](https://docs.microsoft.com/en-us/azure/architecture/patterns/retry).