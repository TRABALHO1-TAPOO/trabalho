using Avalonia.Controls;
using System;
using diario_saude.ViewModels;
using System.Linq;
using Avalonia.Interactivity;

namespace diario_saude.Views
{
    public partial class ReportAndChartsPageView : UserControl
    {
        
        public ReportAndChartsPageViewModel vm => (ReportAndChartsPageViewModel)DataContext;
        public ReportAndChartsPageView()
        {
            InitializeComponent();
            updateButton.Click += UpdateButton_Click;
        }

        private void UpdateButton_Click(object? sender, RoutedEventArgs e)
        {
            var periodo = calendar.SelectedDates.ToList();
 
           /* start.Text = periodo.Min().ToString("dd/MM/yyyy");
            end.Text = periodo.Max().ToString("dd/MM/yyyy");*/
            vm.SelectedDates = periodo;
        }

        
    }
}
