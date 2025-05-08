using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using System.Linq; // Adicionado para corrigir o uso de Where
using ReactiveUI;
using DiarioSaude.Models;
using LinqToDB;

namespace diario_saude.ViewModels
{
    public class DailyRecordPageViewModel : ViewModelBase
    {
        // Propriedades para os campos da tela
        public ObservableCollection<string> MoodOptions { get; } = new ObservableCollection<string>
        {
            "Feliz", "Triste", "Ansioso", "Neutro"
        };

        public ObservableCollection<string> SleepQualityOptions { get; } = new ObservableCollection<string>
        {
            "Boa", "Regular", "Ruim"
        };

        public ObservableCollection<string> PhysicalActivityTypes { get; } = new ObservableCollection<string>
        {
            "Corrida", "Musculação", "Caminhada", "Yoga", "Natação"
        };

        private DateTimeOffset _recordDate = DateTimeOffset.Now;
        public DateTimeOffset RecordDate
        {
            get => _recordDate;
            set
            {
                this.RaiseAndSetIfChanged(ref _recordDate, value);
                ConvertedRecordDate = _recordDate.DateTime; // Converte para DateTime
            }
        }

        private DateTime _convertedRecordDate;
        public DateTime ConvertedRecordDate
        {
            get => _convertedRecordDate;
            private set => this.RaiseAndSetIfChanged(ref _convertedRecordDate, value);
        }

        private string? _selectedMood;
        public string SelectedMood
        {
            get => _selectedMood;
            set => this.RaiseAndSetIfChanged(ref _selectedMood, value);
        }

        private string? _selectedSleepQuality;
        public string SelectedSleepQuality
        {
            get => _selectedSleepQuality;
            set => this.RaiseAndSetIfChanged(ref _selectedSleepQuality, value);
        }

        private string? _foodDescription;
        public string FoodDescription
        {
            get => _foodDescription;
            set => this.RaiseAndSetIfChanged(ref _foodDescription, value);
        }

        private int? _foodCalories;
        public int? FoodCalories
        {
            get => _foodCalories;
            set => this.RaiseAndSetIfChanged(ref _foodCalories, value);
        }


        private int? _physicalActivityDuration;
        public int? PhysicalActivityDuration
        {
            get => _physicalActivityDuration;
            set => this.RaiseAndSetIfChanged(ref _physicalActivityDuration, value);
        }

        private string? _selectedPhysicalActivityType;
        public string SelectedPhysicalActivityType
        {
            get => _selectedPhysicalActivityType;
            set => this.RaiseAndSetIfChanged(ref _selectedPhysicalActivityType, value);
        }

        // Comandos
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        // Construtor
        public DailyRecordPageViewModel()
        {
            SaveCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(SaveRecord), this.WhenAnyValue(
            vm => vm.SelectedMood,
            vm => vm.SelectedSleepQuality,
            vm => vm.FoodDescription,
            vm => vm.FoodCalories,
            vm => vm.PhysicalActivityDuration,
            vm => vm.SelectedPhysicalActivityType,
            (mood, sleepQuality, food, calories, activityDuration, activityType) =>
                !string.IsNullOrWhiteSpace(mood) &&
                !string.IsNullOrWhiteSpace(sleepQuality) &&
                !string.IsNullOrWhiteSpace(food) &&
                calories > 0 &&
                activityDuration > 0 &&
                !string.IsNullOrWhiteSpace(activityType)
            ));

            CancelCommand = ReactiveCommand.Create(CancelRecord);
        }

        // Método para salvar o registro
        private async void SaveRecord()
        {
            try
            {
                // Validações
                if (FoodCalories == null || FoodCalories <= 0)
                {
                    Console.WriteLine("Erro: As calorias da alimentação devem ser maiores que zero.");
                    return;
                }

                if (PhysicalActivityDuration == null || PhysicalActivityDuration <= 0)
                {
                    Console.WriteLine("Erro: A duração da atividade física deve ser maior que zero.");
                    return;
                }

                // Caminho do banco de dados
                string connectionString = $"Data Source={App.DbPath}";
                using var db = new DiarioSaudeDb(connectionString);

                // Inserir na tabela Alimentacao
                var alimentacao = new Alimentacao
                {
                    Descricao = FoodDescription,
                    Calorias = FoodCalories.Value // Usa o valor não nulo
                };
                await db.InsertAsync(alimentacao);

                // Consultar o ID gerado para Alimentacao
                var alimentacaoId = await db.Alimentacoes
                    .Where(a => a.Descricao == FoodDescription && a.Calorias == FoodCalories)
                    .Select(a => a.Id)
                    .FirstOrDefaultAsync();

                // Inserir na tabela AtividadeFisica
                var atividadeFisica = new AtividadeFisica
                {
                    TipoAtividade = SelectedPhysicalActivityType,
                    DuracaoMinutos = PhysicalActivityDuration.Value // Usa o valor não nulo
                };
                await db.InsertAsync(atividadeFisica);

                // Consultar o ID gerado para AtividadeFisica
                var atividadeFisicaId = await db.AtividadesFisicas
                    .Where(a => a.TipoAtividade == SelectedPhysicalActivityType && a.DuracaoMinutos == PhysicalActivityDuration)
                    .Select(a => a.Id)
                    .FirstOrDefaultAsync();

                // Inserir na tabela RegistroDiario
                var registro = new RegistroDiario
                {
                    Data = ConvertedRecordDate.Date,
                    HumorId = MoodOptions.IndexOf(SelectedMood) + 1,
                    SonoId = SleepQualityOptions.IndexOf(SelectedSleepQuality) + 1,
                    AlimentacaoId = alimentacaoId,
                    AtividadeFisicaId = atividadeFisicaId
                };
                await db.InsertAsync(registro);

                // Exibe uma mensagem de sucesso
                Console.WriteLine("Registro salvo com sucesso!");
            }
            catch (Exception ex)
            {
                // Exibe uma mensagem de erro detalhada
                Console.WriteLine($"Erro ao salvar o registro: {ex.Message}");
            }
            finally
            {
                // Limpa os campos
                RecordDate = DateTimeOffset.Now;
                SelectedMood = null;
                SelectedSleepQuality = null;
                FoodDescription = string.Empty;
                FoodCalories = 0;
                PhysicalActivityDuration = 0;
                SelectedPhysicalActivityType = null;
            }
        }

        // Método para cancelar o registro
        private void CancelRecord()
        {
            // Limpa os campos
            RecordDate = DateTimeOffset.Now;
            SelectedMood = null;
            SelectedSleepQuality = null;
            FoodDescription = string.Empty;
            FoodCalories = 0;
            PhysicalActivityDuration = 0;
            SelectedPhysicalActivityType = null;
        }

    }
}