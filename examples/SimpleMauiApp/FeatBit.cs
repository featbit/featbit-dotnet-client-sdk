using FeatBit.Sdk.Client;
using FeatBit.Sdk.Client.Model;
using FeatBit.Sdk.Client.Options;

namespace SimpleMauiApp;

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

            var options = new FbOptionsBuilder(Secret)
                // set the pollingInterval to 5 seconds for testing purposes
#if ANDROID
                // check the documentation at
                // https://learn.microsoft.com/en-us/dotnet/maui/data-cloud/local-web-services?view=net-maui-8.0
                .Polling(new Uri("http://10.0.2.2:5100"), TimeSpan.FromSeconds(5))
                .Event(new Uri("http://10.0.2.2:5100"))
#else
                .Polling(new Uri("http://localhost:5100"), TimeSpan.FromSeconds(5))
                .Event(new Uri("http://localhost:5100"))
#endif
                .Build();

            _instance = new FbClient(options, initialUser: InitialUser);

            // Start the client and wait for 3 seconds to initialize.
            _instance.StartAsync(TimeSpan.FromSeconds(3)).Wait();

            return _instance;
        }
    }

    public static async Task LoginAsync(FbUser authorizedUser)
        => await Instance.IdentifyAsync(authorizedUser);

    public static async Task LogoutAsync(FbUser? anonymousUser = null)
        => await Instance.IdentifyAsync(anonymousUser ?? InitialUser);
}