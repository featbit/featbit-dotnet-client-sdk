﻿// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using System.Text.Json.Serialization;
using FeatBit.ClientSdk;
using FeatBit.ClientSdk.Model;
using FeatBit.ClientSdk.Options;
using Microsoft.Extensions.Logging;

var consoleLoggerFactory = LoggerFactory.Create(opt => opt.AddConsole().SetMinimumLevel(LogLevel.Debug));

var options = new FbOptionsBuilder("JbmetT2IvU2CJTxObJLbiQ1XEjhWE6kEaf1IbJu7gTNQ")
    .Polling(new Uri("http://localhost:5100"), TimeSpan.FromSeconds(10))
    .LoggerFactory(consoleLoggerFactory)
    .Build();

var user = FbUser.Builder("tester").Build();

var fbClient = new FbClient(options, user);
if (fbClient.Initialized)
{
    Console.WriteLine("Client initialized");
}
else
{
    Console.WriteLine("Client failed to initialized");
}

var allFlags = fbClient.AllFlags();

var serializerOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    NumberHandling = JsonNumberHandling.AllowReadingFromString,
    WriteIndented = true
};
var json = JsonSerializer.Serialize(allFlags, serializerOptions);
Console.WriteLine(json);

Console.ReadKey();