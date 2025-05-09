using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;
using LinqToDB;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.IO;
using Avalonia.Metadata;


namespace diario_saude.ViewModels
{
    public class ReportAndChartsPageViewModel : ViewModelBase
    {

        private String _startFood;
        public String StartFood
        {
            get => _startFood;
            set => this.RaiseAndSetIfChanged(ref _startFood, value);
        }

        private String _endFood;
        public String EndFood
        {
            get => _endFood;
            set => this.RaiseAndSetIfChanged(ref _endFood, value);
        }


        // Propriedade para armazenar as datas selecionadas
        private List<DateTime> _selectedDates;
        public List<DateTime> SelectedDates
        {
            get => _selectedDates;
            set => this.RaiseAndSetIfChanged(ref _selectedDates, value);
        }

        // Propriedades para armazenar as datas de início e fim
        private DateTime _startDate;
        public DateTime StartDate
        {
            get => _startDate;
            set => this.RaiseAndSetIfChanged(ref _startDate, value);
        }

        // Propriedade para armazenar a data de fim
        private DateTime _endDate;
        public DateTime EndDate
        {
            get => _endDate;
            set => this.RaiseAndSetIfChanged(ref _endDate, value);
        }


        public ReportAndChartsPageViewModel()
        {
            // Sempre que SelectedDates mudar, cai aqui:
            this.WhenAnyValue(vm => vm.SelectedDates)
                .Subscribe(newCollection =>
                {
                    // 'periodo' acabou de ser atribuído a SelectedDates
                    // Coloque aqui a lógica que você quer disparar
                    LoadRecordsFromDatabase().ContinueWith(_ => OnPeriodoChanged(newCollection));
                });
        }

        // Método que será chamado quando SelectedDates mudar
        private void OnPeriodoChanged(List<DateTime> periodo)
        {
            // Exemplo de acionamento:
            var all = periodo?.ToList() ?? new List<DateTime>();
            StartDate = all.Any() ? all.Min() : DateTime.Today;
            EndDate   = all.Any() ? all.Max() : DateTime.Today;

            StartFood = FilteredRecords.FirstOrDefault()?.FoodDescription ?? "Desconhecido";
            EndFood = FilteredRecords.LastOrDefault()?.FoodDescription ?? "Desconhecido";

        }


        public ObservableCollection<Record> Records { get; } = new ObservableCollection<Record>();
        public ObservableCollection<Record> FilteredRecords { get; set; } = new ObservableCollection<Record>();

        public async Task LoadRecordsFromDatabase()
        {
            try
            {
                var recordsFromDb = await FetchRecordsFromDatabaseAsync();

                // Atualiza as coleções na UI thread
                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    Records.Clear();

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
                        Id = registro.Id,
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
            Log($"Aplicando filtro de {StartDate:d} até {EndDate:d}");

            Log($"Total de registros em Records antes do filtro: {Records.Count}");
            FilteredRecords.Clear();

            // Filtra por período entre StartDate e EndDate (inclusive)
            var filtered = Records
                .Where(r =>
                    DateTime.TryParse(r.Date, out var date)
                    && date.Date >= StartDate.Date
                    && date.Date <= EndDate.Date
                ); // :contentReference[oaicite:0]{index=0}

            Log($"Total de registros em Records após criar o IEnumerable filtrado: {Records.Count}");

            foreach (var record in filtered)
            {
                Log($"Registro filtrado: Data={record.Date}, Humor={record.Mood}, Sono={record.SleepQuality}, " +
                    $"Alimentação={record.FoodDescription}, Calorias={record.FoodCalories}, " +
                    $"Atividade={record.Activity}, Duração={record.Duration}");

                FilteredRecords.Add(record);
            }

            this.RaisePropertyChanged(nameof(FilteredRecords));
        }

        public void Log(string message)
        {
            var logFilePath = "log.txt"; // Caminho do arquivo de log
            File.AppendAllText(logFilePath, $"{DateTime.Now}: {message}{Environment.NewLine}");
        }

    }
}
