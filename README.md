# Dodo.HttpClientExtensions library

![master](https://github.com/dodopizza/httpclientextensions/workflows/master/badge.svg)

The main goal of this library is to provide unified http request retrying policies for the HttpClient that just works.

Actually this library wraps awesome [Polly](https://github.com/App-vNext/Polly) liberary with the predifined settings to allow developers to use it as is without a deep dive to Polly.

The `DefaultPolicy` provided by this library combines `RetryPolicy`, `CircuitBreakerPolicy` and `TimeoutPolicy` under the hood. See the corresponding sections of the README.

## Functionality provided by the library

Library provides few methods which returns the IHttpClientBuilder and you may chain it with other HttpMessageHandler.

There are list of public methods to use:

```csharp
// Default policies for a single host environment using all defaults
IHttpClientBuilder AddDefaultPolicies(this IHttpClientBuilder clientBuilder);

// Default policies for a single host environment with custom settings
IHttpClientBuilder AddDefaultPolicies(this IHttpClientBuilder clientBuilder, HttpClientSettings settings);

// Default policies for a multi host environments using all defaults
IHttpClientBuilder AddDefaultHostSpecificPolicies(this IHttpClientBuilder clientBuilder);

// Default policies for a multi host environments with custom settings
IHttpClientBuilder AddDefaultHostSpecificPolicies(this IHttpClientBuilder clientBuilder, HttpClientSettings settings);

// Default JsonClient includes DefaultPolicies with custom settings
IHttpClientBuilder AddJsonClient<TClientInterface, TClientImplementation>(
    this IServiceCollection sc,
    Uri baseAddress,
    HttpClientSettings settings,
    string clientName = null)
        where TClientInterface : class
        where TClientImplementation : class, TClientInterface
```

There are also available `HttpClientSettings`, `IRetrySettings` and `ICircuitBreakerSettings` to tune-in the default policies. See the corresponding sections of the README.

## HttpClient configuration

You have two options how to add HttpClient in your code.

1. Just use default client provided by the library and add it to the `ServiceCollection` in the Startup like this:

```csharp
service                     // IServiceCollection
    .AddJsonClient(...)     // Default client with policies
```

2. You may add your own HttpClient and then add default policies. In this case it is important to configure Timeout property in the client:

```csharp
service                     // IServiceCollection
    .AddHttpClient("named-client",
        client =>
        {
            client.Timeout = TimeSpan.FromMilliseconds(Defaults.Timeout.HttpClientTimeoutInMilliseconds); // Constant provided by the library
        }))
    .AddDefaultPolicies()   // Default policies provided by the library
```

Or if you use custom HttpClientSettings you may get client timeout value from the `HttpClientSettings.HttpClientTimeout` property instead of constant.

Configure `HttpClient.Timeout` is important because HttpClient will use default value of 100 seconds without this configuration. `AddJsonClient` provided by the library is already pre-configured.

More details about TimeoutPolicy in the corresponding section of the README.

## Single host versus multi host environments

You may notice that there are two group of methods:
`DefaultPolicy` for single host environment and `DefaultHostSpecificPolicy` for multi host environments.

The single host environment means that our HttpClient send requests to a single host (the uri of host is never changed). It also means that if the CircuitBreaker will be opened, **all** requests to this host will be stopped for the duration of break.

In the other hand in multi host environment we suppose that we use single client against multiple hosts. For example in the "country agnostic service" scenario when we use a single HttpClient to send requests against the several host for different countries with the same URL pattern like: `ru-host`, `us-host`, `ng-host`, etc. We can't use `DefaultPolicy` as with single host environment scenario. If the CircuitBreaker will be opened on the one host, ex. `ru-host`, all requests to all other hosts will be stopped too, because of the single HttpClient. `DefaultHostSpecificPolicy` handles this situation by "memorizing" the distinct hosts and policies will match requests to the specific hosts to avoid such situations.

## Retry policy

The retry policy handles the situation when the http request fails because of transient error and reties the attempt to complete the request.

The library provides interface `IRetrySettings` to setup retry policy. There are two predefined implementations provided:
- `SimpleRetrySettings` which by default using [Exponential backoff](https://github.com/App-vNext/Polly/wiki/Retry#exponential-backoff) exponentially increase retry times for each attempt.
- `JitterRetrySettings` _(used by default)_ which is exponential too but used [JitterBackoff](https://github.com/App-vNext/Polly/wiki/Retry-with-jitter) to slightly modify retry times to prevent the situation when all of the requests will be attempt in the same time.

The most important parameter in the retry policy is `RetryCount` which means each request may have at most `RetryCount + 1` attempts: initial request and all the retries in case of fail.

You also may implement your own policy settings by implement the `IRetrySettings`. Also you may check the default values in the `Defaults` class.

## CircuitBreaker Policy

Circuit breaker's goal is to prevent requests to the server if it doesn't answer for a while to mostly of the requests. In practice the reason to have a circuit breaker is to prevent requests when server is down or overloaded.

CircuitBreaker has several importatnt parameters:

- `FailureThreshold` means what percentage of failed requests should be for the CircuitBreaker to open.
- `MinimumThroughput` the minimum amount of the requests should be for the CircuitBreaker to open.
- `DurationOfBreak` amount of time when the CircuitBreaker prevents all the requests to the host.
- `SamplingDuration` during this amount of time CircuitBreaker will count success/failed requests and check two parameters above to make a decision should it opens or not.

[More information about Circuit Breakers in the Polly documentation](https://github.com/App-vNext/Polly/wiki/Advanced-Circuit-Breaker).


The library provides interface `ICircuitBreakerSettings` to setup circuit breaker policy and default implementation `CircuitBreakerSettings` which has a several constructors to tune-in parameters above.

You also may implement your own policy settings by implement the `ICircuitBreakerSettings`. Also you may check the default values in the `Defaults` class.

## Timeout policy

The timeout policy cancels requests in case of long responses (server doesn't response for a long time).

There are only two settings to configure the timeouts:

- `HttpClientTimeout` which set the timeout to the whole HttpClient.
- `TimeoutPerTry` which set the timeout for a single request attempt.

Understanding of the difference between this two parameters is very important to create robust policies.

`HttpClientTimeout` is set to the whole HttpClient. Actually it set `HttpClient.Timeout` property. When this timeout exceeded the HttpClient throws `TaskCancelledException` which prevent all requests in the current session. Such a timeout will not be retried even if not all retry attempts have been made.

`TimeoutPerTry` just setup the timeout for a single request. If this timeout exceeded request will be cancelled and retried even if the server worked correctly and finally response with 200 status code.

Notice that the `HttpClientTimeout` should be **greater** than `TimeoutPerRetry` otherwise you requests will never be retried.

One more important thing is the order of the policies. `TimeoutPolicy` should always be **after** the RetryPolicy otherwise the `TimeoutPerRetry` paramater will play the same role as a `HttpClientTimeout`. [Clarification from the Polly documentation](https://github.com/App-vNext/Polly/wiki/Polly-and-HttpClientFactory#use-case-applying-timeouts).

You may setup your own timeout parameters by providing it to the `HttpClientSettings` constructor. Also you may check the default values in the `Defaults` class.

## Library versions

Library using [SemVer](https://semver.org/) for versioning. If you are working on the new version of library you should update the value of the `VersionPrefix` tag in the `Dodo.HttpClientExtensions.csproj`.

Version change policy (from the SemVer):
> Given a version number `MAJOR.MINOR.PATCH`, increment the:
>
> - `MAJOR` version when you make incompatible API changes.
> - `MINOR` version when you add functionality in a backwards compatible manner.
> - `PATCH` version when you make backwards compatible bug fixes.

## Build

Here is [drone](https://drone.dodois.ru/dodopizza/httpclientextensions) build of the library. Build occurs for each commit to any branch.

## Publish the NuGet package

To publish the library you should use Drone-CLI promote command:

```bash
drone build promote dodopizza/httpclientextensions <build_number> myget
```

The last argument stands for the environment name. In case of library you may provide any value there because it is not used.

You may publish library as the stable or as pre-release NuGet package.

Publishing from the `master` branch considered as stable and will have version equals to the version in the `VersionPrefix` tag from the csproj.
Publishing from other branches considered as pre-release and will have a postfix with the branch name.

For example, if the value of the `VersionPrefix` equals to `4.0.0`. Publish from `master` branch will publish the NuGet package with version `4.0.0`.
If we have a branch from current `master` with the name `my-branch` and publish package from this branch you will have the pre-release package with version `4.0.0-my-branch`.
