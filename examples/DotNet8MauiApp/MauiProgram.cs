using Microsoft.Extensions.Logging;
using FeatBit.ClientSdk;
using Microsoft.Extensions.Logging.Abstractions;

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

            var options = new FbOptionsBuilder("S37S_0bmkUKTQkCIg5GnKQ5ZjgdjXPU0qDo5LAVn4GzA")
                                .Eval(new Uri("https://app-eval.featbit.co"))
                                .LoggerFactory(NullLoggerFactory.Instance)
                                .DataSyncMethod(DataSyncMethodEnum.Polling, 300000)
                                .Build();
            builder.Services.AddSingleton<IFbClient>(provider => new FbClient(options, autoSync: false));

            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<MainViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
