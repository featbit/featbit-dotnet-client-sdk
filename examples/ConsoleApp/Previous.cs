using FeatBit.Sdk.Client;
using FeatBit.Sdk.Client.Model;
using FeatBit.Sdk.Client.Options;
using Microsoft.Extensions.Logging;

namespace ConsoleApp;

public class Previous
{
    public static async Task RunAsync()
    {
        var consoleLoggerFactory = LoggerFactory.Create(opt => opt.AddConsole());
        var options = new FbOptionsBuilder("JbmetT2IvU2CJTxObJLbiQ1XEjhWE6kEaf1IbJu7gTNQ")
            .Polling(new Uri("http://localhost:5100"), TimeSpan.FromSeconds(10))
            .LoggerFactory(consoleLoggerFactory)
            .Build();

        var anonymousUser = FbUser.Builder("anonymous").Build();
        var fbClient = new FbClient(options, anonymousUser);

        Console.WriteLine("Input a user name:");
        var userName = Console.ReadLine() ?? "unknown";

        var authenticatedUser = FbUser.Builder($"key-{userName.Trim().Replace(" ", "-")}")
            .Name(userName)
            .Custom("custom property", "custom value")
            .Build();
        await fbClient.IdentifyAsync(authenticatedUser);

        Console.ReadKey();
        fbClient.Dispose();
    }
}