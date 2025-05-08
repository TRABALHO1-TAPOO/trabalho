using Avalonia.Controls;

namespace diario_saude.Views
{
    public partial class EditRecordWindow : Window
    {
        public EditRecordWindow(object viewModel)
        {
            InitializeComponent();
            DataContext = viewModel; // Configura o DataContext com o ViewModel passado
        }
    }
}