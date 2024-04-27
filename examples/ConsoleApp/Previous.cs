using FeatBit.ClientSdk;
using FeatBit.ClientSdk.Model;
using FeatBit.ClientSdk.Options;
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

        // TODO: Uncomment the following code to run multi-thread tasks
        // Console.WriteLine("Press any key to run multi-thread tasks");
        // Console.ReadKey();
        //
        // var tasks = new Task[5];
        // for (var i = 0; i < tasks.Length; i++)
        // {
        //     tasks[i] = Task.Run(() =>
        //     {
        //         fbClient.StringVariation("welcome-text-visibility", "Collapsed");
        //         return Task.Delay(10 * 000);
        //     });
        // }

        // await Task.WhenAll(tasks);

        Console.ReadKey();
        fbClient.Dispose();
    }
}