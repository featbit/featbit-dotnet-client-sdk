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
                                .Eval(new Uri("https://featbit-tio-eval.zeabur.app"))
                                .LoggerFactory(NullLoggerFactory.Instance)
                                .DataSyncMethod(DataSyncMethodEnum.Polling, 30000)
                                .Build();
            builder.Services.AddSingleton<IFbClient>(provider => new FbClient(options));

            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<MainViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
