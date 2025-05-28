using System.Text.Json;
using FeatBit.Sdk.Client;
using FeatBit.Sdk.Client.ChangeTracker;
using FeatBit.Sdk.Client.Model;
using FeatBit.Sdk.Client.Options;
using Microsoft.Extensions.Logging;

// Set secret to your FeatBit SDK secret.
const string secret = "";
if (string.IsNullOrWhiteSpace(secret))
{
    Console.WriteLine("Please edit Program.cs to set secret to your FeatBit SDK secret first. Exiting...");
    Environment.Exit(1);
}

// Creates a new client to connect to FeatBit with a custom option.
// use console logging for FbClient
var consoleLoggerFactory = LoggerFactory.Create(opt => opt.AddConsole().SetMinimumLevel(LogLevel.Information));

var options = new FbOptionsBuilder(secret)
    // set polling interval to 10s for testing purpose
    .Polling(new Uri("http://localhost:5100"), TimeSpan.FromSeconds(10))
    .Event(new Uri("http://localhost:5100"))
    .LoggerFactory(consoleLoggerFactory)
    .Build();

var initialUser = FbUser.Builder("tester-id")
    .Name("tester")
    .Custom("role", "developer")
    .Build();

var client = new FbClient(options, initialUser);

// Starts the client and wait for 3 seconds to initialize.
var success = await client.StartAsync(TimeSpan.FromSeconds(3));
if (!success)
{
    Console.WriteLine("FbClient failed to initialize. Exiting...");
    Environment.Exit(-1);
}

Console.WriteLine($"Current user is '{initialUser.Name}'");

// Get all flag values for the initial user.
var serializerOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true
};

Console.WriteLine($"All flag values for {initialUser.Name}:");
Console.WriteLine(JsonSerializer.Serialize(client.AllFlags(), serializerOptions));

// Track flag value changes
Subscriber generalSubscriber = changeEvent =>
{
    Console.WriteLine(
        "This is generalSubscriber for all flags. Flag value for '{0}' has changed from '{1}' to '{2}'",
        changeEvent.Key,
        changeEvent.OldValue,
        changeEvent.NewValue
    );
};
var flagTracker = client.FlagTracker;
flagTracker.Subscribe(generalSubscriber);

Subscriber keyedSubscriber = changeEvent =>
{
    Console.WriteLine(
        "This is gameRunnerSubscriber for 'game-runner' flag only. Flag value for 'game-runner' has changed from '{0}' to '{1}'",
        changeEvent.OldValue,
        changeEvent.NewValue
    );
};
flagTracker.Subscribe("game-runner", keyedSubscriber);

// Identify user
Console.WriteLine("After 1s, we will identify a new user 'authorized'...");
await Task.Delay(1000);

var authorizedUser = FbUser.Builder("authorized-id").Name("authorized").Build();
await client.IdentifyAsync(authorizedUser);
Console.WriteLine($"Current user changed to '{authorizedUser.Name}'");

// Evaluate flags for current user
Console.WriteLine("Please input flagKey, for example 'use-new-algorithm'.");

var flagKey = Console.ReadLine();
var detail = client.StringVariationDetail(flagKey, "fallback");

Console.WriteLine("Value for flag '{0}' is '{1}', reason: {2}", flagKey, detail.Value, detail.Reason);

// delay 1s to ensure that all events has been sent
await Task.Delay(1000);
client.Dispose();