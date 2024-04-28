// See https://aka.ms/new-console-template for more information

using FeatBit.Sdk.Client;
using FeatBit.Sdk.Client.ChangeTracker;
using FeatBit.Sdk.Client.Model;
using FeatBit.Sdk.Client.Options;
using Microsoft.Extensions.Logging;

// Set secret to your FeatBit SDK secret.
const string secret = "JbmetT2IvU2CJTxObJLbiQ1XEjhWE6kEaf1IbJu7gTNQ";
if (string.IsNullOrWhiteSpace(secret))
{
    Console.WriteLine("Please edit Program.cs to set secret to your FeatBit SDK secret first. Exiting...");
    Environment.Exit(1);
}

// Creates a new client to connect to FeatBit with a custom option.
// use console logging for FbClient
var consoleLoggerFactory = LoggerFactory.Create(opt => opt.AddConsole().SetMinimumLevel(LogLevel.Debug));

var options = new FbOptionsBuilder(secret)
    .Polling(new Uri("http://localhost:5100"), TimeSpan.FromSeconds(10))
    .Event(new Uri("http://localhost:5100"))
    .LoggerFactory(consoleLoggerFactory)
    .Build();

var initialUser = FbUser.Builder("tester-id")
    .Name("tester")
    .Custom("role", "developer")
    .Build();

var client = new FbClient(options, initialUser);
if (!client.Initialized)
{
    Console.WriteLine("FbClient failed to initialize. Exiting...");
    Environment.Exit(-1);
}

Subscriber generalSubscriber = changeEvent =>
{
    Console.WriteLine(
        "This is generalSubscriber for all flags. Flag '{0}' has changed from '{1}' to '{2}'",
        changeEvent.Key,
        changeEvent.OldValue,
        changeEvent.NewValue
    );
};
client.FlagTracker.Subscribe(generalSubscriber);

Subscriber gameRunnerSubscriber = changeEvent =>
{
    Console.WriteLine(
        "This is gameRunnerSubscriber for 'game-runner' flag only. Flag value for 'game-runner' has changed from '{0}' to '{1}'",
        changeEvent.OldValue,
        changeEvent.NewValue
    );
};
client.FlagTracker.Subscribe("game-runner", gameRunnerSubscriber);
// client.FlagTracker.Unsubscribe("game-runner", gameRunnerSubscriber);

while (true)
{
    Console.WriteLine("Please input flagKey, for example 'use-new-algorithm'. Input 'exit' to exit.");

    var flagKey = Console.ReadLine();
    if (flagKey == "exit")
    {
        Console.WriteLine("Exiting, please wait...");
        break;
    }

    var detail = client.StringVariationDetail(flagKey, "fallback");
    Console.WriteLine("Value for flag '{0}' is '{1}', reason: {2}", flagKey, detail.Value, detail.Reason);
    Console.WriteLine();
}