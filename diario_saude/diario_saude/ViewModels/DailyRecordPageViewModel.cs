using System;
using System.Collections.ObjectModel;
using System.Reactive;
using ReactiveUI;

namespace diario_saude.ViewModels
{
    public class DailyRecordPageViewModel : ViewModelBase
    {
        // Propriedades para os campos da tela
        public ObservableCollection<string> MoodOptions { get; } = new ObservableCollection<string>
        {
            "Feliz", "Triste", "Ansioso", "Calmo", "Cansado"
        };

        public ObservableCollection<string> SleepQualityOptions { get; } = new ObservableCollection<string>
        {
            "Excelente", "Boa", "Regular", "Ruim", "Péssima"
        };

        public ObservableCollection<string> PhysicalActivityTypes { get; } = new ObservableCollection<string>
        {
            "Corrida", "Musculação", "Caminhada", "Yoga", "Natação"
        };

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

        private int _physicalActivityDuration;
        public int PhysicalActivityDuration
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
            SaveCommand = ReactiveCommand.Create(SaveRecord, this.WhenAnyValue(
                vm => vm.SelectedMood,
                vm => vm.SelectedSleepQuality,
                vm => vm.FoodDescription,
                vm => vm.PhysicalActivityDuration,
                vm => vm.SelectedPhysicalActivityType,
                (mood, sleepQuality, food, activityDuration, activityType) =>
                    !string.IsNullOrWhiteSpace(mood) &&
                    !string.IsNullOrWhiteSpace(sleepQuality) &&
                    !string.IsNullOrWhiteSpace(food) &&
                    activityDuration > 0 &&
                    !string.IsNullOrWhiteSpace(activityType)
            ));

            CancelCommand = ReactiveCommand.Create(CancelRecord);
        }

        // Método para salvar o registro
        private void SaveRecord()
        {
            // Aqui você pode implementar a lógica para persistir os dados no SQLite
            Console.WriteLine("Registro salvo com sucesso!");
            Console.WriteLine($"Humor: {SelectedMood}");
            Console.WriteLine($"Qualidade do Sono: {SelectedSleepQuality}");
            Console.WriteLine($"Descrição da Alimentação: {FoodDescription}");
            Console.WriteLine($"Tipo de Atividade Física: {SelectedPhysicalActivityType}");
            Console.WriteLine($"Duração da Atividade Física: {PhysicalActivityDuration} minutos");
        }

        // Método para cancelar o registro
        private void CancelRecord()
        {
            // Limpa os campos
            SelectedMood = null;
            SelectedSleepQuality = null;
            FoodDescription = string.Empty;
            PhysicalActivityDuration = 0;
            SelectedPhysicalActivityType = null;
        }
    }
}