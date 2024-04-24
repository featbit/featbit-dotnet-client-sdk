# FeatBit Client-Side SDK for .NET

> [!WARNING]
> **Client SDK is still under development, we aim to release the first version in the end of the April or early May**. Please watch the repository to get the latest updates.


## Introduction

This is the .NET Client-Side SDK for the 100% open-source feature flags management platform [FeatBit](https://github.com/featbit/featbit).

The FeatBit Client-Side SDK for .NET is designed primarily for use in single-user application such as web, mobile, or desktop applications. It's not suitable for server-side applications.

<!-- ## Data Synchronization

Currently, the client-side SDK uses polling mode to synchronize data with the FeatBit server. You can set the polling interval when initializing the SDK. The SDK also provides a method to manually synchronize data to keep the data up to date in custom scenarios.

You can load feature flags from a local file. You can also export the latest flags values to your local file and use it as the data source when initializing the SDK. This feature is useful when:

- You want to use the SDK in offline mode, especially when a desktop application is running without an internet connection.
- You want to bootstrap the SDK with saved feature flags data to reduce the time to get the latest feature flags evaluation result. -->

## Getting Started

This section will only introduce the basic usage of the SDK. For more detailed use case for mobile, web and desktop applications, please refer to the [Examples](#examples) sections.

### Installation

The latest stable version is available on [NuGet]().

```bash
dotnet add package FeatBit.ClientSdk
```

Use the --version option to specify a [preview version](https://www.nuget.org/packages/FeatBit.ClientSdk/absoluteLatest) to install.

### Prerequisite

Before using the SDK, you need to obtain the environment secret and SDK URLs.

Follow the documentation below to retrieve these values

- [How to get the environment secret](https://docs.featbit.co/sdk/faq#how-to-get-the-environment-secret)
- [How to get the SDK URLs](https://docs.featbit.co/sdk/faq#how-to-get-the-sdk-urls)

### Quick Start

The following code demonstrates basic usage of FeatBit.ClientSdk.


```csharp
using FeatBit.ClientSdk;

var options = new FbOptionsBuilder("<replace-with-your-env-client-secret>")
                    .Eval(new Uri("<replace-with-your-event-url>"))
                    .DataSyncMethod(DataSyncMethodEnum.Polling, 10000)
                    .Build();
var fbClient = new FbClient(options, autoSync: true);

// callback when feature flags are updated
fbClient.FeatureFlagsUpdated += (object? sender, FeatureFlagsUpdatedEventArgs e) =>
{
    foreach (var ff in e.UpdatedFeatureFlags)
    {
        // ff.Id is the feature flag key
        // ff.Variation is the evaluated result for the feature flag
        Console.WriteLine($"{ff.Id}: {ff.Variation}"); 
    }
};

// After login or user identification, call IdentifyAsync to update the user information 
// and retrieve the latest feature flags evalution result for this user.
var user = FbUser.Builder("<a-unique-key-of-user>")
                 .Name("<name-of-user>")
                 .Custom("<custom-property-1>", "<custom-value>")
                 .Custom("<custom-property-2>", "<custom-value>")
                 .Build();
await fbClient.IdentifyAsync(user);

// evaluate a boolean flag for a the user
var boolVariation = client.BoolVariation("<feature-flag-key>", defaultValue: false);
Console.WriteLine($"flag '{flagKey}' returns {boolVariation} for user {user.Key}");

// close the client
await fbClient.DisposeAsync();
```

### Examples

- [Asp.Net Core Blazor WebAssembly](https://github.com/featbit/featbit-dotnet-client-sdk/tree/main/examples/BlazorClient/WebAssemblyApp)
- [Console APP](https://github.com/featbit/featbit-dotnet-client-sdk/tree/main/examples/ConsoleApp)
- [.NET MAUI](https://github.com/featbit/featbit-dotnet-client-sdk/tree/main/examples/DotNet8MauiApp)
- [WPF](https://github.com/featbit/featbit-dotnet-client-sdk/tree/main/examples/WPFApp)


## SDK

### FbClient

The FbClient is the core component of the SDK, providing access to the FeatBit server. I recommend that applications treat it as a **singleton instance** for the lifetime of the application. You can create multiple instances of the client-side SDK in the same application for custom purposes.

#### Initialize FbClient with FbOptions.

```csharp
using FeatBit.ClientSdk;

var options = new FbOptionsBuilder("<replace-with-your-env-client-secret>")
                    .Eval(new Uri("<replace-with-your-event-url>")) // in SaaS mode, it's https://app-eval.featbit.co
                    .DataSyncMethod(DataSyncMethodEnum.Polling, 10000)
                    .Build();

// Creates a new client instance that connects to FeatBit with the custom option.
var fbClient = new FbClient(options, autoSync: true);
```

#### Dependency Injection

This SDK is designed for multiple .NET front-end application platforms. Although we do not provide built-in support for dependency injection, you can use any DI container to manage the FbClient instance. In the examples, I demonstrate how to register the FbClient as a singleton service in Blazor WebAssembly, WPF, and MAUI applications.

```csharp
var options = new FbOptionsBuilder("<replace-with-your-env-client-secret>")
                    .Eval(new Uri("<replace-with-your-event-url>")) // in SaaS mode, it's https://app-eval.featbit.co
                    .DataSyncMethod(DataSyncMethodEnum.Polling, 30000)
                    .Build();
builder.Services.AddSingleton<IFbClient>(new FbClient(options));
```

### Data Synchronization 

Currently, the client side SDK uses polling mode to synchronize data with the FeatBit server. You can set the interval of the polling when initializing the SDK. SDK provides also a method to manually synchronize data to keep the data up-to-date in custom scenarios.

#### Polling Mode

If you set `autoSync` to `true`, the SDK will start the data synchronization (polling mode) automatically. You can set the interval by changing the second parameter of `DataSyncMethod` method. For example, if you want to sync data every 5 minutes, you can set it to 300000.

```csharp
var options = new FbOptionsBuilder("<replace-with-your-env-client-secret>")
                    .Eval(new Uri("<replace-with-your-event-url>"))
                    .DataSyncMethod(DataSyncMethodEnum.Polling, 300000)
                    .Build();
```

You can stop the data synchronization whenever you want by calling `StopAutoData`.

```csharp
var fbClient = new FbClient(options, autoSync: true);
fbClient.StopAutoData();
```

#### Manual Mode

If you don't want to start the data synchronization automatically, you can set `autoSync` to `false`.

```csharp
var fbClient = new FbClient(options, autoSync: false);
```

If you want to sync data manually, you can use `UpdateToLatestAsync` method.

```csharp
await fbClient.UpdateToLatestAsync();
```

When you want to start the data synchronization, you can call `StartAutoData` method.

```csharp
fbClient.StartAutoData();
```

### User Identification

By default, a anonymous user is created when the SDK is initialized. After login or user identification, you can update the user information by calling `Identify` or `IdentifyAsync` method. Before calling method, you need to create a user object by using `FbUser.Builder` method.

```csharp
var user = FbUser.Builder("<a-unique-key-of-user>")
                 .Name("<name-of-user>")
                 .Custom("<custom-property-1>", "<custom-value>")
                 .Custom("<custom-property-2>", "<custom-value>")
                 .Build();
```

NOTE: custom value can be any type of object but wrapped in a string. For example, if it's a double 22.3, you should use "22.3" as the value.

By calling `Identify`, the user information will be updated immediately, but the feature flags will not be updated immediately until the next data synchronization (triggered by the polling interval or method `UpdateToLatestAsync`).

```csharp
fbClient.Identify(user);
```

By calling `IdentifyAsync`, the user information and the feature flags evaluation will be updated immediately.

```csharp   
await fbClient.IdentifyAsync(user);
```
> [!NOTE]
> If `IdentifyAsync` is called at the same time that the polling interval triggers, a `SemaphoreSlim` will let the second call wait until the first call is finished. User information will always be updated to the latest one before the second call is executed.

### Evaluating flags

By using the feature flag data it has already received, the SDK get the value of a feature flag for a given user from local memory. To evaluate a feature flag, you can call the following methods.

```csharp
bool boolVariation = fbClient.BoolVariation("<feature-flag-key>", defaultValue: false);
int intVariation = fbClient.IntVariation("<feature-flag-key>", defaultValue: 0);
float floatVariation = fbClient.FloatVariation("<feature-flag-key>", defaultValue: 0);
double doubleVariation = fbClient.DoubleVariation("<feature-flag-key>", defaultValue: 0);
string stringVariation = fbClient.StringVariation("<feature-flag-key>", defaultValue: "");
```

The `defaultValue` parameter is optional. If the feature flag is not found, the SDK will return the default value.

### Track Feature Usage Insights

Each time a `Variation` method is called for evaluating a feature flag, the SDK will automatically track the feature usage insights. 

If you don't need to track the usage insight of a feature flag, you can set the `trackInsight` parameter to `false`. Following code demonstrates how to disable tracking for a feature flag.

```csharp
bool boolVariation = fbClient.BoolVariation("<feature-flag-key>", 
                                            defaultValue: false, 
                                            trackInsight: false);
```

### Experimentation Metric

You can track the experiment metric by calling the `Track` method.  **Current version does not support the `Track` method.**

### Bootstrap

If you want to bootstrap the SDK with saved feature flags data to reduce the time to get the latest feature flags evaluation result. You can:

1. Retrieve your feature flags data from a local file or a custom data source.
2. Use the `InitFeatureFlagsFromLocal` method to update the evaluated feature flags result data.

```csharp
// code to read feature flags from local file or data service
// to List<FeatureFlag> type object, then
fbClient.InitFeatureFlagsFromLocal(featureFlagsData);
```

You can also export the latest flags values to your local file and then use it as the data source when initializing the SDK.

```csharp
var featureFlags = fbClient.GetLatestAll();
// code to save featureFlags to a local file
```

### Offline Mode

In some scenarios, application need to be used without an internet connection. You can do the same as the bootstrap process to:

1. Load feature flags from a local file.
2. Save the latest flags values to your local file and use it as the data source when initializing the SDK. 

### Offline Mode

In some scenarios, application need to be used without an internet connection. You can load feature flags from a local file. You can also export the latest flags values to your local file and use it as the data source when initializing the SDK.

## Supported .NET versions

This SDK should compatible with any other platform that supports .NET Standard version 2.0 or higher.

## Getting support

- If you have a specific question about using this sdk, we encourage you to [ask it in our slack](https://featbit.slack.com/join/shared_invite/zt-1ew5e2vbb-x6Apan1xZOaYMnFzqZkGNQ).
- If you encounter a bug or would like to request a feature, [submit an issue](https://github.com/featbit/featbit/issues/new).

## See Also

- [Connect To .NET Sdk](https://docs.featbit.co/getting-started/connect-an-sdk#net)