using Avalonia.Controls;
using System;
using diario_saude.ViewModels;
using System.Linq;
using Avalonia.Interactivity;
using System.Collections;
using ScottPlot;
using System.IO;
using System.Collections.ObjectModel;

namespace diario_saude.Views
{
    public partial class ReportAndChartsPageView : UserControl
    {
        
        public ReportAndChartsPageViewModel vm;
        public int i = 0;
        public ObservableCollection<Record_v> minhaColecao { get; set; } = new ObservableCollection<Record_v>();
        public ReportAndChartsPageView()
        {
            InitializeComponent();

            calendar.DisplayDateEnd = DateTime.Today;

            vm = new ReportAndChartsPageViewModel();
            updateButton.Click += UpdateButton_Click;

            // Quando a View estiver carregada, acesse a coleção
            this.AttachedToVisualTree += (_, __) =>
            {
                if (vm is ReportAndChartsPageViewModel)
                {
                    i++;
                    // Agora você tem acesso direto à ObservableCollection
                    minhaColecao = vm.FilteredRecords;

                    // Exemplo: subscribe a mudanças
                    minhaColecao.CollectionChanged += (s, e) =>
                    {
                        Log($"MinhaColeção alterada: {e.Action}");

                        if (minhaColecao.Count > 0)
                        {
                            // Aqui você pode fazer algo com a coleção
                            PlotCaloriesVsDays();
                        }
                        
                        // Aqui você pode lidar com a mudança na coleção

                    };
                }
            };


        }

        private void UpdateButton_Click(object? sender, RoutedEventArgs e)
        {   
            var periodo = calendar.SelectedDates.ToList();
            
            vm.SelectedDates = periodo;

        }

        public void PlotCaloriesVsDays()
        {
            // Limpa o gráfico atual
            AvaPlot1.Plot.Clear();
            // Cria um novo gráfico
            Plot myPlot = AvaPlot1.Plot;

            // Adicione os dados ao gráfico
            var dates = minhaColecao.Select(r => DateTime.Parse(r.Date)).ToArray();
            var calories = minhaColecao.Select(r => r.FoodCalories).ToArray();

            // Adiciona os dados ao gráfico como barras
            
            myPlot.Add.Scatter(dates, calories);

            // Define os rótulos do eixo X como as datas formatadas
            myPlot.Axes.DateTimeTicksBottom();

            // Customize plot style
            myPlot.YLabel("Calories");
            myPlot.XLabel("Dates");
            myPlot.Axes.Margins(bottom: 0.2);

            // Refresh the plot
            AvaPlot1.Refresh();

            // Log the plot creation  
            Log($"Gráfico criado com {minhaColecao.Count} registros.");
        }

        public void Log(string message)
        {
            var logFilePath = "log.txt"; // Caminho do arquivo de log
            File.AppendAllText(logFilePath, $"[Reports&Charts&View]{DateTime.Now}: {message}{Environment.NewLine}");
        }
        
    }
}
