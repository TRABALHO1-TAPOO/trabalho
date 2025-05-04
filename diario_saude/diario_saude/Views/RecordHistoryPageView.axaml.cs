using Avalonia.Controls;
using diario_saude.ViewModels;

namespace diario_saude.Views;

public partial class RecordHistoryPageView : UserControl
{
    public RecordHistoryPageView()
    {
        InitializeComponent();
        DataContext = new RecordHistoryPageViewModel(); 
    }
}