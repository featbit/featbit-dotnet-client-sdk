using FeatBit.Sdk.Client;
using FeatBit.Sdk.Client.Model;
using FeatBit.Sdk.Client.Options;
using Microsoft.Extensions.Logging;

namespace DotNet8MauiApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            var options = new FbOptionsBuilder("<replace-with-your-secret>")
                .Polling(new Uri("https://app-eval.featbit.co"))
                .Event(new Uri("https://app-eval.featbit.co"))
                .Build();

            var initialUser = FbUser.Builder("tester-id")
                .Name("tester")
                .Custom("role", "developer")
                .Build();

            var fbClient = new FbClient(options, initialUser);
            builder.Services.AddSingleton<IFbClient>(fbClient);

            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<MainViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}