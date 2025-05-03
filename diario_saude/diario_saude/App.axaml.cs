using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using diario_saude.ViewModels;
using diario_saude.Views;
using System;
using System.IO;
using System.Threading.Tasks;
using DiarioSaude.Models;
using diario_saude.Services;

namespace diario_saude
{
    public partial class App : Application
    {
        public static string DbPath { get; private set; } = "diariosaude.db";
        
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            
            try
            {
                // Configura o caminho do banco de dados no diretório do aplicativo
                string appDataPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "DiarioSaude");
                    
                // Cria o diretório se não existir
                if (!Directory.Exists(appDataPath))
                    Directory.CreateDirectory(appDataPath);
                    
                DbPath = Path.Combine(appDataPath, "diariosaude.db");
                
                // Inicializa o banco de dados
                string connectionString = $"Data Source={DbPath}";
                DiarioSaudeDb.CreateDatabase(connectionString);
                
                // Inicializa o ThemeService de forma assíncrona
                Task.Run(async () => await ThemeService.InitializeAsync(connectionString)).Wait();
                
                Console.WriteLine($"Banco de dados inicializado em: {DbPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inicializar o banco de dados: {ex.Message}");
            }
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}