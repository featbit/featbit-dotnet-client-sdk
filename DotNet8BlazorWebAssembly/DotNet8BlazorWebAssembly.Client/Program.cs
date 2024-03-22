using FeatBit.ClientSdk;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

await builder.Build().RunAsync();


var consoleLoggerFactory = LoggerFactory.Create(x => x.AddConsole());
var options = new FbOptionsBuilder("s0ZIrZbMfEuZGv3UrDtskAUVG0I5KKrEyYAqZtyI-5IQ")
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

if (fbClient.Initialized)
{
    Console.WriteLine("Client is initialized");
    Console.WriteLine("testing-visibility:" + fbClient.StringVariation("testing-visibility", "Collapsed"));
}
else
{
    Console.WriteLine("Client is not initialized");
}