# FeatBit Client-Side SDK for .NET

## Introduction

This is the .NET Client-Side SDK for the 100% open-source feature flags management platform FeatBit.

The FeatBit Client-Side SDK for .NET is designed primarily for use in single-user application such as web, mobile, or desktop applications. It is a lightweight SDK that the evaluation of feature flags is done on the remote server or local pre-configured file.

## Data Synchronization

Currently, the client side SDK uses polling mode to synchronize data with the FeatBit server. You can set the interval of the polling when initializing the SDK. SDK provides also a method to manually synchronize data to keep the data up-to-date in custom scenarios.

You can use your own data source to initialize the SDK.


## Getting Started

This section will only introduce the basic usage of the SDK. For more detailed use case for mobile, web and desktop applications, please refer to the next sections.

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
    if (e.UpdatedFeatureFlags.Count > 0)
    {
        Console.WriteLine("Feature flags updated:");
        foreach (var ff in e.UpdatedFeatureFlags)
        {
            // ff.Id is the feature flag key
            // ff.Variation is the evaluated result for the feature flag
            Console.WriteLine($"{ff.Id}: {ff.Variation}"); 
        }
    }
    else
    {
        Console.WriteLine("No feature flags updated.");
    }
};

// After login or user identification, call IdentifyAsync to update the user information and retrieve the latest feature flags evalution result for this user.
var user = FbUser.Builder("<a-unique-key-of-user>")
                 .Name("<name-of-user>")
                 .Custom("<custom-property-1>", "<custom-value>")
                 .Custom("<custom-property-2>", "<custom-value>")
                 .Build();
await fbClient.IdentifyAsync(user);
// or use synchronous method. This will update user information but will not retrieve the latest feature flags evaluation result immediately.
// fbClient.Identify(user);

// evaluate a boolean flag for a the user
var boolVariation = client.BoolVariation("<feature-flag-key>", defaultValue: false);
Console.WriteLine($"flag '{flagKey}' returns {boolVariation} for user {user.Key}");

// initialize the SDK with a custom data source
fbClient.InitFeatureFlagsFromLocal(new List<FeatureFlag>() {  
    // you can init a collection from local file or other data source
});

// manually sync data to keep the data up-to-date
await fbClient.UpdateToLatestAsync();

// stop the auto data sync in polling mode
fbClient.StopAutoData();
// restart the auto data sync in polling mode
fbClient.StartAutoData();

// dispose the client
await fbClient.DisposeAsync();
```

### Examples

