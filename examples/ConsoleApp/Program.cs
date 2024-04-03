// See https://aka.ms/new-console-template for more information
using FeatBit.ClientSdk;
using Microsoft.Extensions.Logging;

Console.WriteLine("Hello, World!");


var consoleLoggerFactory = LoggerFactory.Create(x => x.AddConsole());
var options = new FbOptionsBuilder("S37S_0bmkUKTQkCIg5GnKQ5ZjgdjXPU0qDo5LAVn4GzA")
                    .Eval(new Uri("https://featbit-tio-eval.zeabur.app"))
                    .LoggerFactory(consoleLoggerFactory)
                    .PollingInterval(5000)
                    .Build();
var fakeUser = FbUser.Builder("a-unique-key-of-fake-user-001")
                .Name("Fake User 001")
                .Custom("age", "15")
                .Custom("country", "FR")
                .Build();
var fbClient = new FbClient(options, fbUser: fakeUser, autoSync: true);

fbClient.FeatureFlagsUpdated += (sender, e) =>
{
    if(e.UpdatedFeatureFlags.Count > 0)
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

Console.WriteLine("Press any key to simulate user login - 002");
Console.ReadKey();
Console.WriteLine("Key Pressed");

fbClient.StopAutoDataSync();
var fakeUser2 = FbUser.Builder("a-unique-key-of-fake-user-002")
                .Name("Fake User 002")
                .Custom("age", "18")
                .Custom("country", "US")
                .Build();
fbClient.Identify(fakeUser2);
fbClient.StartAutoDataSync();

Console.WriteLine("Press any key to simulate user login - 001");
Console.ReadKey();
Console.WriteLine("Key Pressed");

fbClient.StopAutoDataSync();
fbClient.Identify(fakeUser);
fbClient.StartAutoDataSync();

Console.ReadLine();

await fbClient.CloseAsync();

