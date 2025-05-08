using System;
using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using System.Reactive;
using System.Threading.Tasks;
using DiarioSaude.Models; 
using LinqToDB;
using System.Collections.Generic;
using System.IO;

namespace diario_saude.ViewModels
{
    public class RecordHistoryPageViewModel : ViewModelBase
    {
        public ObservableCollection<Record> Records { get; } = new ObservableCollection<Record>();
        public ObservableCollection<Record> FilteredRecords { get; set; } = new ObservableCollection<Record>();
        public ObservableCollection<string> FilterOptions { get; } = new ObservableCollection<string>
        {
            "Diário", "Semanal", "Mensal"
        };

        private string _selectedFilter = "Mensal"; // Inicializa com um valor padrão
        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedFilter, value);
                ApplyFilter(); // Aplica o filtro automaticamente ao alterar o valor
            }
        }

        public ReactiveCommand<Unit, Unit> ApplyFilterCommand { get; }
        public ReactiveCommand<Record, Unit> EditCommand { get; }
        public ReactiveCommand<Record, Unit> DeleteCommand { get; }

        public RecordHistoryPageViewModel()
        {
            ApplyFilterCommand = ReactiveCommand.Create(ApplyFilter);
            EditCommand = ReactiveCommand.Create<Record>(EditRecord);
            DeleteCommand = ReactiveCommand.Create<Record>(DeleteRecord);

            // Carrega os registros do banco de dados
            LoadRecordsFromDatabase();
        }

        public async Task LoadRecordsFromDatabase()
        {
            try
            {
                var recordsFromDb = await FetchRecordsFromDatabaseAsync();

                // Atualiza as coleções na UI thread
                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    Records.Clear();
                    FilteredRecords.Clear();

                    foreach (var record in recordsFromDb)
                    {
                        Records.Add(record);
                    }

                    Log($"Registros carregados em Records: {Records.Count}");
                    Log($"Registros carregados em FilteredRecords: {FilteredRecords.Count}");
                });

                // Notifica a interface sobre as alterações
                this.RaisePropertyChanged(nameof(FilteredRecords));
            }
            catch (Exception ex)
            {
                Log($"Erro ao carregar registros do banco de dados: {ex.Message}");
            }

            // Aplica o filtro inicial
            ApplyFilter();
        }

        private async Task<ObservableCollection<Record>> FetchRecordsFromDatabaseAsync()
        {
            var records = new ObservableCollection<Record>();

            try
            {
                using var repository = new DiarioSaudeRepository($"Data Source={App.DbPath}");

                // Obtenha todos os registros da tabela RegistroDiario
                var registrosDiarios = await Task.Run(() => repository.ObterRegistrosDiariosAsync());
                Log($"Registros carregados do banco: {registrosDiarios.Count}");

                // Obtenha todas as descrições das tabelas relacionadas
                var humores = await repository.ObterHumoresAsync();
                var qualidadesSono = await repository.ObterQualidadesSonoAsync();
                var alimentacoes = await repository.ObterAlimentacoesAsync();
                var atividadesFisicas = await repository.ObterAtividadesFisicasAsync();

                // Converta os registros para o modelo Record
                foreach (var registro in registrosDiarios)
                {
                    var humorDescricao = humores.FirstOrDefault(h => h.Id == registro.HumorId)?.Descricao ?? "Desconhecido";
                    var sonoDescricao = qualidadesSono.FirstOrDefault(s => s.Id == registro.SonoId)?.Descricao ?? "Desconhecido";
                    var alimentacaoDescricao = alimentacoes.FirstOrDefault(a => a.Id == registro.AlimentacaoId)?.Descricao ?? "Desconhecido";
                    var alimentacaoCalorias = alimentacoes.FirstOrDefault(a => a.Id == registro.AlimentacaoId)?.Calorias ?? 0;
                    var atividadeDuracao = atividadesFisicas.FirstOrDefault(a => a.Id == registro.AtividadeFisicaId)?.DuracaoMinutos ?? 0;
                    var atividadeDescricao = atividadesFisicas.FirstOrDefault(a => a.Id == registro.AtividadeFisicaId)?.TipoAtividade ?? "Desconhecido";

                    Log($"Registro encontrado: Data={registro.Data}, Humor={humorDescricao}, Sono={sonoDescricao}, Alimentação={alimentacaoDescricao}, Atividade={atividadeDescricao}");

                    records.Add(new Record
                    {
                        Date = registro.Data.ToShortDateString(),
                        Mood = humorDescricao,
                        SleepQuality = sonoDescricao,
                        FoodDescription = alimentacaoDescricao,
                        FoodCalories = alimentacaoCalorias,
                        Activity = atividadeDescricao,
                        Duration = atividadeDuracao
                    });
                }
            }
            catch (Exception ex)
            {
                Log($"Erro ao buscar registros do banco de dados: {ex.Message}");
            }

            return records;
        }

        private void ApplyFilter()
        {
            Log($"Aplicando filtro: {SelectedFilter}");

            Log($"Total de registros em Records antes do filtro1: {Records.Count}");
            FilteredRecords.Clear();

            Log($"Total de registros em Records antes do filtro2: {Records.Count}");

            var filtered = SelectedFilter switch
            {
                "Diário" => Records.Where(r => DateTime.TryParse(r.Date, out var date) && date >= DateTime.Now.Date),
                "Semanal" => Records.Where(r => DateTime.TryParse(r.Date, out var date) && date >= DateTime.Now.AddDays(-7)),
                "Mensal" => Records.Where(r => DateTime.TryParse(r.Date, out var date) && date >= DateTime.Now.AddMonths(-1)),
                _ => Records
            };

            Log($"Total de registros em Records antes do filtro3: {Records.Count}");
            foreach (var record in filtered)
            {
                // Adiciona os registros filtrados ao log
                Log($"Registro filtrado: Data={record.Date}, Humor={record.Mood}, Sono={record.SleepQuality}, Alimentacao={record.FoodDescription}, Calorias={record.FoodCalories} Atividade={record.Activity}, Duracao={record.Duration}");

                FilteredRecords.Add(record);
            }

            this.RaisePropertyChanged(nameof(FilteredRecords));
        }


        private void EditRecord(Record record)
        {
            // Lógica para editar o registro
            Console.WriteLine($"Editar registro: {record.Date}");
        }

        private void DeleteRecord(Record record)
        {
            // Lógica para excluir o registro
            Records.Remove(record);
            ApplyFilter(); // Reaplica o filtro após excluir
        }

        public void Log(string message)
        {
            var logFilePath = "log.txt"; // Caminho do arquivo de log
            File.AppendAllText(logFilePath, $"{DateTime.Now}: {message}{Environment.NewLine}");
        }
    }

    public class Record
    {
        public string? Date { get; set; }
        public string? Mood { get; set; }
        public string? FoodDescription { get; set; }
        public int FoodCalories { get; set; }
        public string? SleepQuality { get; set; }
        public string? Activity { get; set; }
        public int Duration { get; set; }
    }

    public class DiarioSaudeRepository : IDisposable
    {
        private readonly string _connectionString;

        public DiarioSaudeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<RegistroDiario>> ObterRegistrosDiariosAsync()
        {
            using var db = new DiarioSaudeDb(_connectionString);
            return await db.RegistroDiario.ToListAsync();
        }

        public async Task<List<Humor>> ObterHumoresAsync()
        {
            using var db = new DiarioSaudeDb(_connectionString);
            return await db.Humores.ToListAsync();
        }

        public async Task<List<QualidadeSono>> ObterQualidadesSonoAsync()
        {
            using var db = new DiarioSaudeDb(_connectionString);
            return await db.QualidadesSono.ToListAsync();
        }

        public async Task<List<Alimentacao>> ObterAlimentacoesAsync()
        {
            using var db = new DiarioSaudeDb(_connectionString);
            return await db.Alimentacoes.ToListAsync();
        }

        public async Task<List<AtividadeFisica>> ObterAtividadesFisicasAsync()
        {
            using var db = new DiarioSaudeDb(_connectionString);
            return await db.AtividadesFisicas.ToListAsync();
        }

        public void Dispose()
        {
            // Libera recursos, se necessário
        }
    }
}