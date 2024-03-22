// See https://aka.ms/new-console-template for more information
using FeatBit.ClientSdk;
using Microsoft.Extensions.Logging;

Console.WriteLine("Hello, World!");


var consoleLoggerFactory = LoggerFactory.Create(x => x.AddConsole());
var options = new FbOptionsBuilder("xxx")
                    .Event(new Uri("https://featbit-tio-eu-eval.azurewebsites.net"))
                    .Streaming(new Uri("wss://featbit-tio-eu-eval.azurewebsites.net"))
                    .APIs(new Uri("https://featbit-tio-eu-api.azurewebsites.net"))
                    .LoggerFactory(consoleLoggerFactory)
                    .StartWaitTime(TimeSpan.FromSeconds(10))
                    .Build();

var fbClient = new FbClient(options);

var fakeUser = FbUser.Builder("a-unique-key-of-fake-user-001")
                .Name("Fake User 001")
                .Custom("age", "15")
                .Custom("country", "FR")
                .Build();
fbClient.Identify(fakeUser);

if(fbClient.Initialized)
{
    Console.WriteLine("Client is initialized");
    Console.WriteLine("testing-visibility:" + fbClient.StringVariation("testing-visibility", "Collapsed"));
}
else
{
    Console.WriteLine("Client is not initialized");
}


await fbClient.CloseAsync();

