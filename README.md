# FeatBit Client-Side SDK for .NET

## Introduction

This is the .NET Client-Side SDK for the 100% open-source feature flags management
platform [FeatBit](https://github.com/featbit/featbit).

Be aware, this is a client side SDK, it is intended for use in a single-user context, which can be mobile, desktop or
embedded applications. It is not intended for use in multi-user systems such as web servers.

For using FeatBit in *server-side* .NET applications, refer to
our [Server-Side .NET SDK](https://github.com/featbit/featbit-dotnet-sdk).

## Getting Started

### Installation

The latest stable version is available on [NuGet](https://www.nuget.org/packages/FeatBit.ClientSdk/).

```bash
dotnet add package FeatBit.ClientSdk
```

Use the `--version` option to specify
a [preview version](https://www.nuget.org/packages/FeatBit.ClientSdk/absoluteLatest)
to install.

### Prerequisite

Before using the SDK, you need to obtain the environment secret and SDK URLs.

Follow the documentation below to retrieve these values

- [How to get the environment secret](https://docs.featbit.co/sdk/faq#how-to-get-the-environment-secret)
- [How to get the SDK URLs](https://docs.featbit.co/sdk/faq#how-to-get-the-sdk-urls)

### Quick Start

The following code demonstrates some basic usages of FeatBit.ClientSdk including:

- Initializing the SDK
- Identifying a user
- Evaluating a feature flag
- Subscribing to the changes of a feature flag

```csharp
using FeatBit.Sdk.Client;
using FeatBit.Sdk.Client.Model;
using FeatBit.Sdk.Client.Options;

// setup SDK options
var options = new FbOptionsBuilder("<replace-with-your-env-secret>")
    .Polling(new Uri("<replace-with-your-polling-url>"), TimeSpan.FromMinutes(5))
    .Event(new Uri("<replace-with-your-event-url>"))
    .Build();

// use the anonymous user as the initial user
var anonymousUser = FbUser.Builder("anonymous")
    .Name("anonymous")
    .Custom("role", "visitor")
    .Build();

// Creates a new client instance that connects to FeatBit with the custom option.
var client = new FbClient(options, anonymousUser);
if (!client.Initialized)
{
    Console.WriteLine("FbClient failed to initialize. All Variation calls will use fallback value.");
}
else
{
    Console.WriteLine("FbClient successfully initialized!");
}

// after user logged in, call IdentifyAsync to switch the user and get the latest feature flags for the user
var authenticatedUser = FbUser.Builder("a-unique-key-of-bob")
    .Name("bob")
    .Custom("country", "FR")
    .Build();
await client.IdentifyAsync(authenticatedUser);

// flag to be evaluated
const string flagKey = "game-runner";

// evaluate a boolean flag for the user
var boolVariation = client.BoolVariation(flagKey, defaultValue: false);
Console.WriteLine($"flag '{flagKey}' returns {boolVariation} for user {authenticatedUser.Key}");

// evaluate a boolean flag for the user with reason
var boolVariationDetail = client.BoolVariationDetail(flagKey, defaultValue: false);
Console.WriteLine(
    $"flag '{flagKey}' returns {boolVariationDetail.Value} for user {authenticatedUser.Key}. " +
    $"Reason Description: {boolVariationDetail.Reason}"
);

// subscribe to flag changes
var flagTracker = client.FlagTracker;
flagTracker.Subscribe(flagKey, @event =>
{
    Console.WriteLine(
        "Flag value for '{0}' has changed from '{1}' to '{2}'",
        @event.Key,
        @event.OldValue,
        @event.NewValue
    );
});
```

### Examples

- [Console App](https://github.com/featbit/featbit-dotnet-client-sdk/tree/main/examples/ConsoleApp)

## SDK

### Data Synchronization

Currently, the client-side SDK uses polling mode to synchronize data with the FeatBit server. You can set the polling
interval when initializing the SDK. The SDK also provides a method to manually synchronize data to keep the data up to
date in custom scenarios.

### FbClient

The FbClient is the heart of the SDK which providing access to FeatBit server. Applications should instantiate a single
instance for the lifetime of the application.

#### FbClient Using Custom Options

```csharp
using FeatBit.Sdk.Client;
using FeatBit.Sdk.Client.Model;
using FeatBit.Sdk.Client.Options;
using Microsoft.Extensions.Logging;

// use console logging for FbClient
var consoleLoggerFactory = LoggerFactory.Create(opt => opt.AddConsole());

var options = new FbOptionsBuilder(secret)
    .Polling(new Uri("http://localhost:5100"), TimeSpan.FromSeconds(10))
    .Event(new Uri("http://localhost:5100"))
    .StartWaitTime(TimeSpan.FromSeconds(3))
    .LoggerFactory(consoleLoggerFactory)
    .Build();

var initialUser = FbUser.Builder("tester-id")
    .Name("tester")
    .Custom("role", "developer")
    .Build();

// Creates a new client instance that connects to FeatBit with the custom option.
var client = new FbClient(options, initialUser);
```

#### Dependency Injection

This SDK is designed for multiple .NET front-end application platforms. For now there is no built-in support for
dependency injection, you can use any DI container to manage the FbClient instance. For example:

```csharp
var fbClient = new FbClient(options, initialUser);

// register the fbClient instance to the DI container
builder.Services.AddSingleton<IFbClient>(fbClient);
```

### FbUser

FbUser defines the attributes of a user for whom you are evaluating feature flags. FbUser has two built-in
attributes: `key` and `name`. The only mandatory attribute of a FbUser is the key, which must uniquely identify each
user.

Besides these built-in properties, you can define any additional attributes associated with the user
using `Custom(string key, string value)` method on `IFbUserBuilder`. Both built-in attributes and custom attributes can
be referenced in targeting rules, and are included in analytics data.

There is only one method for building FbUser.

```csharp
var bob = FbUser.Builder("a-unique-key-of-bob")
    .Name("bob")
    .Custom("age", "15")
    .Custom("country", "FR")
    .Build();
```

### Track flag changes

To get notified when a feature flag is changed, we offer two methods

- subscribe to the changes of any feature flag(s)
```csharp
Subscriber generalSubscriber = changeEvent =>
{
  Console.WriteLine(
      "This is generalSubscriber for all flags. Flag '{0}' value has changed from '{1}' to '{2}'",
      changeEvent.Key,
      changeEvent.OldValue,
      changeEvent.NewValue
  );
};
flagTracker.Subscribe(generalSubscriber);
```
- subscribe to the changes of a specific feature flag
```csharp
Subscriber keyedSubscriber = changeEvent =>
{
  Console.WriteLine(
      "This is gameRunnerSubscriber for 'game-runner' flag only. Flag value for 'game-runner' has changed from '{0}' to '{1}'",
      changeEvent.OldValue,
      changeEvent.NewValue
  );
};
flagTracker.Subscribe("game-runner", keyedSubscriber);
```

To unsubscribe from the changes of a feature flag, you can call the `Unsubscribe` method.

```csharp
flagTracker.Unsubscribe(generalSubscriber);
flagTracker.Unsubscribe(keyedSubscriber);
```

### Identifying and changing user

Like all client-side FeatBit SDKs, the FbClient always has **a single current user**, which is used to evaluate feature
flags against with. All calls to evaluation methods like `BoolVariation` refer to the flag values for the current user.
You specify the initial user when you create the `FbClient` instance, and then you can change it at any time by calling
the `IdentifyAsync` method, such as when an end user logs in or changes their settings.

```csharp
var authenticatedUser = FbUser.Builder("a-unique-key-of-bob")
    .Name("bob")
    .Custom("country", "FR")
    .Build();

// tells the FbClient to switch to the new user and get the latest feature flags for that user
await client.IdentifyAsync(authenticatedUser);
```

### Evaluating flags

By using the feature flag data it has already received, the SDK get the value of a feature flag for a given user from
memory. To evaluate a feature flag, you can call the following methods.

There is a Variation method that returns a flag value, and a VariationDetail method that returns an object describing
how the value was determined for each type.

* BoolVariation/BoolVariationDetail
* StringVariation/StringVariationDetail
* DoubleVariation/DoubleVariationDetail
* FloatVariation/FloatVariationDetail
* IntVariation/IntVariationDetail
* JsonVariation/JsonVariationDetail (in consideration)

> [!NOTE]
> Since the current version does not have native support for retrieving JSON variations, you can utilize
> the `StringVariation` method as an alternative to obtain the JSON string.

### Offline Mode

In some scenarios, you might want to stop making remote calls to FeatBit or our application need to be used without an
internet connection. In this case, you can set the `Offline` option to `true` when initializing the SDK.

```csharp
var options = new FbOptionsBuilder()
    .Offline(true)
    .Build();

var initialUser = FbUser.Builder("anonymous").Build();

var client = new FbClient(options, initialUser);
```

> [!IMPORTANT]
> When you put the SDK in offline mode, no insight message is sent to the server and all feature flag evaluations will
> return fallback values if you didn't bootstrap the SDK with your own feature flags data.

### Bootstrapping

In some scenarios, you might want to use your own feature flags data to initialize the SDK. For example:

- You want to use the SDK in offline mode, especially when a desktop application is running without an internet
  connection.
- You want to bootstrap the SDK with saved feature flags data to reduce the time to get the feature flags evaluation
  result.

In this case, you can use the `Bootstrap` option:

```csharp
var options = new FbOptionsBuilder()
    .Bootstrap(yourFeatureFlags)
    .Build();
```

You can use the `AllFlags` method to get the latest feature flags data.

```csharp
IDictionary<string, FeatureFlag> featureFlags = fbClient.AllFlags();
```

You can then save the data to a file or a database and use it to bootstrap the SDK in the next application run.

## Supported .NET versions

This SDK should compatible with any other platform that supports .NET Standard version 2.0 or higher.

## Getting support

- If you have a specific question about using this sdk, we encourage you
  to [ask it in our slack](https://featbit.slack.com/join/shared_invite/zt-1ew5e2vbb-x6Apan1xZOaYMnFzqZkGNQ).
- If you encounter a bug or would like to request a
  feature, [submit an issue](https://github.com/featbit/featbit/issues/new).

## See Also

- [Connect To .NET Sdk](https://docs.featbit.co/getting-started/connect-an-sdk#net)
