using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using diario_saude.ViewModels;
using diario_saude.Views;
using System;
using System.IO;
using DiarioSaude.Models;

namespace diario_saude
{
    public partial class App : Application
    {
        public static string DbPath { get; private set; } = "diariosaude.db";
        
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            
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