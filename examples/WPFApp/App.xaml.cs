using FeatBit.ClientSdk;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace WPFApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            var consoleLoggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            var options = new FbOptionsBuilder("*****")
                                .Event(new Uri("https://featbit-tio-eu-eval.azurewebsites.net"))
                                .Streaming(new Uri("wss://featbit-tio-eu-eval.azurewebsites.net"))
                                .APIs(new Uri("https://featbit-tio-eu-api.azurewebsites.net"))
                                .LoggerFactory(consoleLoggerFactory)
                                .Build();
            services.AddSingleton<IFbClient>(provider => new FbClient(options));
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainWindow>();
            services.AddTransient<LoginWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //var mainWindow = _serviceProvider.GetService<MainWindow>();
            //mainWindow.Show();

            var loginWindow = _serviceProvider.GetService<LoginWindow>();
            loginWindow.Show();
        }
    }

}
