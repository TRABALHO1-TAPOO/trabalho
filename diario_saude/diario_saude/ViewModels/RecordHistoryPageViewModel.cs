using System;
using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using System.Reactive;
using System.Threading.Tasks;
using DiarioSaude.Models; 

namespace diario_saude.ViewModels
{
    public class RecordHistoryPageViewModel : ViewModelBase
    {
        public ObservableCollection<Record> Records { get; } = new ObservableCollection<Record>();
        public ObservableCollection<Record> FilteredRecords { get; } = new ObservableCollection<Record>();
        public ObservableCollection<string> FilterOptions { get; } = new ObservableCollection<string>
        {
            "Diário", "Semanal", "Mensal"
        };

        private string _selectedFilter = "Diário"; // Inicializa com um valor padrão
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
            // Carrega os registros do banco de dados
            LoadRecordsFromDatabase();

            ApplyFilterCommand = ReactiveCommand.Create(ApplyFilter);
            EditCommand = ReactiveCommand.Create<Record>(EditRecord);
            DeleteCommand = ReactiveCommand.Create<Record>(DeleteRecord);
        }

        private async void LoadRecordsFromDatabase()
        {
            try
            {
                //Console.WriteLine($"Caminho do banco de dados: {App.DbPath}");

                // Simulação de carregamento de registros do banco de dados
                var recordsFromDb = await FetchRecordsFromDatabaseAsync();

                // Adiciona os registros carregados ao ObservableCollection na UI thread
                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    Records.Clear();
                    foreach (var record in recordsFromDb)
                    {
                        Records.Add(record);
                    }

                    // Inicializa o FilteredRecords com todos os registros
                    FilteredRecords.Clear();
                    foreach (var record in Records)
                    {
                        FilteredRecords.Add(record);
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar registros do banco de dados: {ex.Message}");
            }
        }

        private async Task<ObservableCollection<Record>> FetchRecordsFromDatabaseAsync()
        {
            var records = new ObservableCollection<Record>();

            try
            {
                using var repository = new DiarioSaudeRepository($"Data Source={App.DbPath}");

                // Obtenha todos os registros da tabela RegistroDiario
                var registrosDiarios = await Task.Run(() => repository.ObterRegistrosDiariosAsync());
                Console.WriteLine($"Registros carregados: {registrosDiarios.Count}");

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
                    var atividadeDescricao = atividadesFisicas.FirstOrDefault(a => a.Id == registro.AtividadeFisicaId)?.TipoAtividade ?? "Desconhecido";

                    Console.WriteLine($"Registro encontrado: Data={registro.Data}, Humor={humorDescricao}, Sono={sonoDescricao}, Alimentação={alimentacaoDescricao}, Atividade={atividadeDescricao}");

                    records.Add(new Record
                    {
                        Date = registro.Data.ToShortDateString(),
                        Mood = humorDescricao,
                        SleepQuality = sonoDescricao,
                        Activity = atividadeDescricao,
                        Duration = registro.AtividadeFisicaId // Substitua por lógica para calcular a duração, se necessário
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar registros do banco de dados: {ex.Message}");
            }

            return records;
        }

        private void ApplyFilter()
        {
            FilteredRecords.Clear();

            var filtered = SelectedFilter switch
            {
                "Diário" => Records.Where(r => DateTime.Parse(r.Date) >= DateTime.Now.Date),
                "Semanal" => Records.Where(r => DateTime.Parse(r.Date) >= DateTime.Now.AddDays(-7)),
                "Mensal" => Records.Where(r => DateTime.Parse(r.Date) >= DateTime.Now.AddMonths(-1)),
                _ => Records
            };

            foreach (var record in filtered)
            {
                FilteredRecords.Add(record);
            }
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
    }

    public class Record
    {
        public string? Date { get; set; }
        public string? Mood { get; set; }
        public string? SleepQuality { get; set; }
        public string? Activity { get; set; }
        public int Duration { get; set; }
    }
}