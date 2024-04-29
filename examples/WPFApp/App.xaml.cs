﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Windows;
using FeatBit.Sdk.Client;
using FeatBit.Sdk.Client.Model;
using FeatBit.Sdk.Client.Options;

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

            //CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("fr-FR");
            //CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("fr-FR");
        }

        private void ConfigureServices(IServiceCollection services)
        {
            var consoleLoggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

            var options = new FbOptionsBuilder("S37S_0bmkUKTQkCIg5GnKQ5ZjgdjXPU0qDo5LAVn4GzA")
                .Polling(new Uri("https://featbit-tio-eval.zeabur.app"))
                .LoggerFactory(consoleLoggerFactory)
                .Build();

            var user = FbUser.Builder("tester").Build();
            var fbClient = new FbClient(options, user);

            services.AddSingleton<IFbClient>(fbClient);

            services.AddTransient<MainWindow>();
            services.AddTransient<LoginWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var loginWindow = _serviceProvider.GetService<LoginWindow>();
            loginWindow.Show();
        }
    }
}