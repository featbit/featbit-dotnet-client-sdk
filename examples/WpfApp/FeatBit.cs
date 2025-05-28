using System;
using FeatBit.Sdk.Client;
using FeatBit.Sdk.Client.Model;
using FeatBit.Sdk.Client.Options;
using Microsoft.Extensions.Logging;

namespace WpfApp;

public static class FeatBit
{
    private const string Secret = "";
    private static readonly FbUser InitialUser = FbUser.Builder("anonymous-id").Name("anonymous").Build();
    private static FbClient? _instance;

    public static FbClient Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }

            if (string.IsNullOrWhiteSpace(Secret))
            {
                throw new InvalidOperationException("Please set the FeatBit secret key.");
            }

            var consoleLogger = LoggerFactory.Create(x => x.AddConsole().SetMinimumLevel(LogLevel.Information));
            var options = new FbOptionsBuilder(Secret)
                // set the pollingInterval to 5 seconds for testing purposes
                .Polling(new Uri("http://localhost:5100"), TimeSpan.FromSeconds(5))
                .Event(new Uri("http://localhost:5100"))
                .LoggerFactory(consoleLogger)
                .Build();

            _instance = new FbClient(options, initialUser: InitialUser);

            // Start the client and wait for 3 seconds to initialize.
            _instance.StartAsync(TimeSpan.FromSeconds(3)).Wait();

            return _instance;
        }
    }
}