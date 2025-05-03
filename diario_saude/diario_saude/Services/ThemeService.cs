using System;
using System.Linq;
using System.Threading.Tasks;
using DiarioSaude.Models;
using LinqToDB;
using Avalonia;
using Avalonia.Styling;

namespace diario_saude.Services
{
    public static class ThemeService
    {
        private static string _preference = "claro"; // Valor padrão
        private static DiarioSaudeDb? _db;

        public static async Task InitializeAsync(string connectionString)
        {
            _db = new DiarioSaudeDb(connectionString);

            // Carregar a configuração do banco de dados
            var config = await _db.Configuracoes.FirstOrDefaultAsync();
            if (config != null)
            {
                _preference = config.Tema; // Carregar o tema salvo no banco
            }
            else
            {
                // Criar configuração padrão se não existir
                await _db.InsertAsync(new Configuracao
                {
                    CaminhoBanco = connectionString,
                    Tema = _preference // Salvar o tema padrão
                });
            }

            // Aplicar o tema carregado
            ApplyTheme(_preference);

            // Notificar a interface sobre o tema carregado
            PreferenceChanged?.Invoke(_preference);
        }

        private static void ApplyTheme(string theme)
        {
            if (theme == "claro")
            {
                (Avalonia.Application.Current as Application)!.RequestedThemeVariant = ThemeVariant.Light;
            }
            else
            {
                (Avalonia.Application.Current as Application)!.RequestedThemeVariant = ThemeVariant.Dark;
            }
        }

        public static string Preference
        {
            get => _preference;
            set
            {
                if (_preference == value) return;
                _preference = value;

                // Atualizar o tema no banco de dados
                if (_db != null)
                {
                    var config = _db.Configuracoes.FirstOrDefault();
                    if (config != null)
                    {
                        config.Tema = _preference;
                        _db.Update(config);
                    }
                }

                // Aplicar o tema atualizado
                ApplyTheme(_preference);

                // Notificar os ouvintes sobre a mudança de preferência
                PreferenceChanged?.Invoke(_preference);
            }
        }

        public static event Action<string>? PreferenceChanged;

        public static void Toggle()
        {
            Preference = Preference == "claro" ? "escuro" : "claro";
        }
    }
}
