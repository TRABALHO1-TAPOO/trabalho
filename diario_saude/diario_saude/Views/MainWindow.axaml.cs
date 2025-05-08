using Avalonia.Controls;
using System.Diagnostics;

namespace diario_saude.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Debug.WriteLine("MainWindow constructor called.");
            InitializeComponent();
        }
    }
}