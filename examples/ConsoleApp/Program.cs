// See https://aka.ms/new-console-template for more information
using FeatBit.ClientSdk;
using FeatBit.ClientSdk.Singletons;
using Microsoft.Extensions.Logging;

Console.WriteLine("Hello, World!");


var consoleLoggerFactory = LoggerFactory.Create(x => x.AddConsole());
var options = new FbOptionsBuilder("s0ZIrZbMfEuZGv3UrDtskAUVG0I5KKrEyYAqZtyI-5IQ")
                    .Event(new Uri("https://featbit-tio-eu-eval.azurewebsites.net"))
                    .Streaming(new Uri("wss://featbit-tio-eu-eval.azurewebsites.net"))
                    .APIs(new Uri("https://featbit-tio-eu-api.azurewebsites.net"))
                    .LoggerFactory(consoleLoggerFactory)
                    .Build();

var fbClient = new FbClient(options);

var fakeUser = FbIdentity.Builder("a-unique-key-of-fake-user-001")
                .Name("Fake User 001")
                .Custom("age", "15")
                .Custom("country", "FR")
                .Build();
fbClient.Identify(fakeUser);
await fbClient.LoadLatestCollectionFromRemoteServerAsync();

Console.WriteLine("testing-visibility:" + fbClient.StringVariation("testing-visibility", "Collapsed"));

await fbClient.CloseAsync();

