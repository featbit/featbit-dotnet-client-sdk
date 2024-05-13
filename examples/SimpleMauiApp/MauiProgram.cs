using Microsoft.Extensions.Logging;

namespace SimpleMauiApp;

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

        // initialize FbClient by accessing the Instance property
        _ = FeatBit.Instance;

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}