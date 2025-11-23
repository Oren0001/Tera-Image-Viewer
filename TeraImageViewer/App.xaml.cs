using System;
using System.Net.Http;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TeraImageViewer.Services;
using TeraImageViewer.ViewModels;
using TeraImageViewer.Views;

namespace TeraImageViewer {
    public partial class App : Application {
        private IServiceProvider _serviceProvider;

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            ShowLoginWindow();
        }

        private void Application_Startup(object sender, StartupEventArgs e) {
        }

        private void ConfigureServices(IServiceCollection services) {
            services.AddLogging(builder => {
                builder.AddDebug();
                builder.SetMinimumLevel(LogLevel.Debug);
            });

            var httpClient = new HttpClient {
                BaseAddress = new Uri("https://assignment-server-rv-866595813231.us-central1.run.app"),
            };
            services.AddSingleton(httpClient);

            services.AddSingleton<TokenStorage>();
            services.AddSingleton<AuthenticationService>();
            services.AddSingleton<TeraApiService>();
            services.AddSingleton<DataPollingService>();

            services.AddTransient<LoginViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<LoginWindow>();
            services.AddTransient<MainWindow>();
        }

        public void ShowLoginWindow() {
            var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();

            if (loginWindow.ShowDialog() == true) {
                ShowMainWindow();
            } else {
                Shutdown();
            }
        }

        private void ShowMainWindow() {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
