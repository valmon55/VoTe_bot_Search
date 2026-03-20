using KFA.Vote_Search.Models;
using KFA.Vote_Search.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace KFA.Vote_Search
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IHost? AppHost { get; private set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((hostContext,  services) => 
                {
                    services.AddDbContext<VoTeBotContext>(options =>
                        options.UseSqlServer(hostContext.Configuration.GetConnectionString("HPConnection")));
                    services.AddTransient<MessageViewModel>();
                    services.AddSingleton<MainWindow>();
                })
                .Build();
        }
        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                await AppHost.StartAsync();

                //// Применяем миграции (если они есть) или гарантируем создание БД
                //using (var scope = AppHost.Services.CreateScope())
                //{
                //    var dbContext = scope.ServiceProvider.GetRequiredService<VoTeBotContext>();
                //    // Для Database-First обычно не требуется, но можно проверить подключение
                //    // await dbContext.Database.EnsureCreatedAsync();
                //}

                var mainWindow = AppHost.Services.GetRequiredService<MainWindow>();
                mainWindow.Show();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при запуске приложения: {ex.Message}\n\n{ex.StackTrace}",
                                "Критическая ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                // Завершаем приложение
                Current.Shutdown();
            }
            base.OnStartup(e);
        }
        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync();
            AppHost.Dispose();
            base.OnExit(e);
        }
    }

}
