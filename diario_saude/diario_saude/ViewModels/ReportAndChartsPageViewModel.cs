using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;
using LinqToDB;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.IO;
using Avalonia.Metadata;
using ScottPlot;
using diario_saude.Views;
using System.Collections.Specialized;


namespace diario_saude.ViewModels
{
    public class ReportAndChartsPageViewModel : ViewModelBase
    {

        // Propriedade para armazenar Dados do banco de dados
        public ObservableCollection<Record_v> Records { get; } = new ObservableCollection<Record_v>();
        public ObservableCollection<Record_v> FilteredRecords { get; set; } = new ObservableCollection<Record_v>();


        private Plot _plot;
        public Plot myPlot
        {
            get => _plot;
            set => this.RaiseAndSetIfChanged(ref _plot, value);
        }


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
            this.WhenAnyValue(vm => vm.SelectedDates)
                .Subscribe(newCollection =>
                {
                    // Verifica se a coleção de datas selecionadas não é nula
                    if (newCollection != null)
                    {
                        // 1) Aguarda o carregamento dos registros

                        // 2) Só depois chama o OnPeriodoChanged
                        OnPeriodoChanged(newCollection);
                        
                    }
                });
        }


        // Método que será chamado quando Update for chamado (Botão "Atualizar")
        private async void OnPeriodoChanged(List<DateTime> periodo)
        { 
            // Exemplo de acionamento:
            var all = periodo?.ToList() ?? new List<DateTime>();
            StartDate = all.Any() ? all.Min() : DateTime.MinValue;
            EndDate   = all.Any() ? all.Max() : DateTime.MinValue;

            // 3) Aguarda o carregamento dos registros
            await LoadRecordsFromDatabase().ConfigureAwait(false);


            StartFood = FilteredRecords.FirstOrDefault()?.FoodDescription ?? "Desconhecido";
            EndFood = FilteredRecords.LastOrDefault()?.FoodDescription ?? "Desconhecido";

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

        private async Task<ObservableCollection<Record_v>> FetchRecordsFromDatabaseAsync()
        {
            var records = new ObservableCollection<Record_v>();

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

                // Converta os registros para o modelo Record_v
                foreach (var registro in registrosDiarios)
                {
                    var humorDescricao = humores.FirstOrDefault(h => h.Id == registro.HumorId)?.Id ?? 0;
                    var sonoDescricao = qualidadesSono.FirstOrDefault(s => s.Id == registro.SonoId)?.Id ?? 0;
                    var alimentacaoDescricao = alimentacoes.FirstOrDefault(a => a.Id == registro.AlimentacaoId)?.Descricao ?? "Desconhecido";
                    var alimentacaoCalorias = alimentacoes.FirstOrDefault(a => a.Id == registro.AlimentacaoId)?.Calorias ?? 0;
                    var atividadeDuracao = atividadesFisicas.FirstOrDefault(a => a.Id == registro.AtividadeFisicaId)?.DuracaoMinutos ?? 0;
                    var atividadeDescricao = atividadesFisicas.FirstOrDefault(a => a.Id == registro.AtividadeFisicaId)?.TipoAtividade ?? "Desconhecido";

                    Log($"Registro encontrado: Data={registro.Data}, Humor={humorDescricao}, Sono={sonoDescricao}, Alimentação={alimentacaoDescricao}, Atividade={atividadeDescricao}");

                    records.Add(new Record_v
                    {
                        Id = registro.Id,
                        Date = registro.Data.ToShortDateString(),
                        Mood =  humorDescricao,
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

            foreach (var date in SelectedDates)
            {
                var records = Records.Where(r =>
                    DateTime.TryParse(r.Date, out var recordDate)
                    && recordDate.Date == date.Date
                );

                if (records.Any()){
                    foreach (var r in records)
                    {
                        Log($"Registro filtrado: Data={r.Date}, Humor={r.Mood}, Sono={r.SleepQuality}, " +
                            $"Alimentação={r.FoodDescription}, Calorias={r.FoodCalories}, " +
                            $"Atividade={r.Activity}, Duração={r.Duration}");

                        FilteredRecords.Add(r);
                    }
                }else
                {
                    FilteredRecords.Add(new Record_v
                    {
                        Id = 0,
                        Date = date.ToShortDateString(),
                        Mood = 0,
                        SleepQuality = 0,
                        FoodDescription = "Desconhecido",
                        FoodCalories = 0,
                        Activity = "Desconhecido",
                        Duration = 0
                    });
                }
            }

            Log($"Total de registros em FilteredRecords após criar o IEnumerable filtrado: {FilteredRecords.Count()}");

            this.RaisePropertyChanged(nameof(FilteredRecords));
        }

        public void Log(string message)
        {
            var logFilePath = "log.txt"; // Caminho do arquivo de log
            File.AppendAllText(logFilePath, $"[Reports&Charts&View&Model]{DateTime.Now}: {message}{Environment.NewLine}");
        }

    }

    public class Record_v
    {
        public int Id { get; set; }
        public string? Date { get; set; }
        public double? Mood { get; set; }
        public string? FoodDescription { get; set; }
        public double? FoodCalories { get; set; }
        public double? SleepQuality { get; set; }
        public string? Activity { get; set; }
        public double? Duration { get; set; }
    }

}
