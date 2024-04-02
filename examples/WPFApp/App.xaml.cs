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
            var options = new FbOptionsBuilder("S37S_0bmkUKTQkCIg5GnKQ5ZjgdjXPU0qDo5LAVn4GzA")
                                .Eval(new Uri("https://featbit-tio-eval.zeabur.app"))
                                .LoggerFactory(consoleLoggerFactory)
                                .PollingInterval(5000)
                                .Build();
            services.AddSingleton<IFbClient>(provider => new FbClient(options, autoSync: false));
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
