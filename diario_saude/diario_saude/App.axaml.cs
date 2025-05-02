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
                
                // Se o banco de dados já existe e está causando problemas, tentar excluir
                if (File.Exists(DbPath))
                {
                    try
                    {
                        // Apenas durante desenvolvimento - em produção, deve-se fazer backup
                        File.Delete(DbPath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Não foi possível excluir o banco de dados existente: {ex.Message}");
                        // Usa caminho alternativo se não puder excluir
                        DbPath = Path.Combine(appDataPath, $"diariosaude_{DateTime.Now.Ticks}.db");
                    }
                }
                
                // Inicializa o banco de dados
                string connectionString = $"Data Source={DbPath}";
                DiarioSaudeDb.CreateDatabase(connectionString);
                
                Console.WriteLine($"Banco de dados inicializado em: {DbPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inicializar o banco de dados: {ex.Message}");
                // Em caso de falha, use uma localização temporária
                DbPath = Path.Combine(Path.GetTempPath(), $"diariosaude_temp_{Guid.NewGuid()}.db");
                string connectionString = $"Data Source={DbPath}";
                try
                {
                    DiarioSaudeDb.CreateDatabase(connectionString);
                    Console.WriteLine($"Banco de dados inicializado em local temporário: {DbPath}");
                }
                catch (Exception innerEx)
                {
                    Console.WriteLine($"Falha crítica no banco de dados: {innerEx.Message}");
                }
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