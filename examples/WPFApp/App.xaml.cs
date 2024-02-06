using Autofac.Features.ResolveAnything;
using Autofac;
using System.Configuration;
using System.Data;
using System.Windows;
using FeatBit.ClientSdk;
using Autofac.Core;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddSingleton<IFbClient, FbClient>();
            // Register other services and view models
            services.AddTransient<MainViewModel>(); // ViewModel
            services.AddTransient<MainWindow>();
            //services.AddTransient<MainWindow>(provider =>
            //    new MainWindow(provider.GetRequiredService<MainViewModel>()));
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }
    }

}
