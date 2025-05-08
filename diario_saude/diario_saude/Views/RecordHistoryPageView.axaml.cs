using Avalonia.Controls;
using diario_saude.ViewModels;

namespace diario_saude.Views;

public partial class RecordHistoryPageView : UserControl
{
    public RecordHistoryPageView()
    {
        InitializeComponent();
        InitializeViewModelAsync();
    }

    private async void InitializeViewModelAsync()
    {
        var viewModel = new RecordHistoryPageViewModel();

        // Configure o DataContext antes de carregar os registros
        DataContext = viewModel;
        viewModel.Log($"DataContext configurado: {DataContext?.GetType().Name}");

        // Aguarde o carregamento dos registros
        await viewModel.LoadRecordsFromDatabase();

        // Verificar os registros carregados no viewModel
        viewModel.Log($"Total de registros no viewModel: {viewModel.Records.Count}");
        viewModel.Log($"Total de registros filtrados no viewModel: {viewModel.FilteredRecords.Count}");
        viewModel.Log($"Filtro selecionado no viewModel: {viewModel.SelectedFilter}");

        foreach (var record in viewModel.FilteredRecords)
        {
            viewModel.Log($"Registro filtrado no DataContext: Data={record.Date}, Humor={record.Mood}, Sono={record.SleepQuality}, Alimentação={record.FoodDescription}, Atividade={record.Activity}, Duração={record.Duration}");
        }
    }
}