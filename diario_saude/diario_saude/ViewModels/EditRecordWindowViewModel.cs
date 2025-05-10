using ReactiveUI;
using System.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using DiarioSaude.Models;
using LinqToDB;
using LinqToDB.Data;
using Avalonia.Controls;
using Avalonia.Threading;

namespace diario_saude.ViewModels
{
    public class EditRecordWindowViewModel : ViewModelBase
    {
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

        private Window _window;

        // Comandos
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }


        // Construtor
        public EditRecordWindowViewModel()
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
            CancelCommand = ReactiveCommand.Create(CancelAction);

        }

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

                // Atualizar a tabela Alimentacao
                var alimentacaoId = await db.Alimentacoes
                    .Where(a => a.Descricao == FoodDescription && a.Calorias == FoodCalories)
                    .Select(a => a.Id)
                    .FirstOrDefaultAsync();

                if (alimentacaoId > 0)
                {
                    // Atualiza o registro existente
                    await db.Alimentacoes
                        .Where(a => a.Id == alimentacaoId)
                        .Set(a => a.Descricao, FoodDescription)
                        .Set(a => a.Calorias, FoodCalories.Value)
                        .UpdateAsync();
                }
                else
                {
                    Console.WriteLine("Erro: Alimentação não encontrada para atualização.");
                    return;
                }

                // Atualizar a tabela AtividadeFisica
                var atividadeFisicaId = await db.AtividadesFisicas
                    .Where(a => a.TipoAtividade == SelectedPhysicalActivityType && a.DuracaoMinutos == PhysicalActivityDuration)
                    .Select(a => a.Id)
                    .FirstOrDefaultAsync();

                if (atividadeFisicaId > 0)
                {
                    // Atualiza o registro existente
                    await db.AtividadesFisicas
                        .Where(a => a.Id == atividadeFisicaId)
                        .Set(a => a.TipoAtividade, SelectedPhysicalActivityType)
                        .Set(a => a.DuracaoMinutos, PhysicalActivityDuration.Value)
                        .UpdateAsync();
                }
                else
                {
                    Console.WriteLine("Erro: Atividade física não encontrada para atualização.");
                    return;
                }

                // Atualizar a tabela RegistroDiario
                var registroId = await db.RegistroDiario
                    .Where(r => r.Data == ConvertedRecordDate.Date)
                    .Select(r => r.Id)
                    .FirstOrDefaultAsync();

                if (registroId > 0)
                {
                    // Atualiza o registro existente
                    await db.RegistroDiario
                        .Where(r => r.Id == registroId)
                        .Set(r => r.HumorId, MoodOptions.IndexOf(SelectedMood) + 1)
                        .Set(r => r.SonoId, SleepQualityOptions.IndexOf(SelectedSleepQuality) + 1)
                        .Set(r => r.AlimentacaoId, alimentacaoId)
                        .Set(r => r.AtividadeFisicaId, atividadeFisicaId)
                        .UpdateAsync();
                }
                else
                {
                    Console.WriteLine("Erro: Registro diário não encontrado para atualização.");
                    return;
                }

                // Exibe uma mensagem de sucesso
                Console.WriteLine("Registro atualizado com sucesso!");
            }
            catch (Exception ex)
            {
                // Exibe uma mensagem de erro detalhada
                Console.WriteLine($"Erro ao atualizar o registro: {ex.Message}");
            }
            finally
            {
                // Fecha a janela após salvar o registro (se necessário)
                Console.WriteLine("Operação concluída.");
                Dispatcher.UIThread.Post(() => _window?.Close());

            }
        }
        private void CancelAction()
        {
            _window?.Close();
        }

        public void SetWindow(Window window)
        {
            _window = window;
        }
    }
}
