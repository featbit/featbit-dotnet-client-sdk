﻿// See https://aka.ms/new-console-template for more information

using FeatBit.ClientSdk;
using FeatBit.ClientSdk.Events;
using FeatBit.ClientSdk.Model;
using FeatBit.ClientSdk.Options;
using Microsoft.Extensions.Logging;

Console.WriteLine("Hello, World!");

var consoleLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });
var options = new FbOptionsBuilder("S37S_0bmkUKTQkCIg5GnKQ5ZjgdjXPU0qDo5LAVn4GzA")
    .Polling(new Uri("https://app-eval.featbit.co"), TimeSpan.FromSeconds(10))
    .LoggerFactory(consoleLoggerFactory)
    .Build();
var fbClient = new FbClient(options, autoSync: true);

fbClient.FeatureFlagsUpdated += (object? sender, FeatureFlagsUpdatedEventArgs e) =>
{
    if (e.UpdatedFeatureFlags.Count > 0)
    {
        Console.WriteLine("Feature flags updated:");
        foreach (var ff in e.UpdatedFeatureFlags)
        {
            Console.WriteLine($"{ff.Id}: {ff.Variation}");
        }
    }
    else
    {
        Console.WriteLine("No feature flags updated.");
    }
};

Console.WriteLine("Input a user name:");
var userName = Console.ReadLine() ?? "unknown";
var user = FbUser.Builder($"key-{userName.Trim().Replace(" ", "-")}")
    .Name(userName)
    .Custom("custom property", "custom value")
    .Build();
await fbClient.IdentifyAsync(user);

Console.WriteLine("Press any key to run multi-thread tasks");
Console.ReadKey();

var tasks = new Task[5];
for (var i = 0; i < tasks.Length; i++)
{
    tasks[i] = Task.Run(() =>
    {
        fbClient.StringVariation("welcome-text-visibility", "Collapsed");
        fbClient.FeatureFlagsUpdated += (object? sender, FeatureFlagsUpdatedEventArgs e) =>
        {
            Console.WriteLine($"{e.UpdatedFeatureFlags.Count} Feature flags updated in task {i}:");
        };
        Thread.Sleep(10000);
    });
}

await Task.WhenAll(tasks);

Console.ReadKey();

await fbClient.CloseAsync();