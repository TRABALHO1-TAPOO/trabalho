using Avalonia.Controls;
using System;
using diario_saude.ViewModels;
using System.Linq;
using Avalonia.Interactivity;
using System.Collections;
using ScottPlot;
using System.IO;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Tmds.DBus.Protocol;

namespace diario_saude.Views
{
    public partial class ReportAndChartsPageView : UserControl
    {
        
        private double?[] data;
        private string DataType;

        private double average;
        private double total;
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

            var selecionado = DataTypeGroup
                .Children
                .OfType<RadioButton>()
                .FirstOrDefault(rb => rb.IsChecked == true);
            
            Results.IsVisible = true;

            if (selecionado != null)
            {

                switch (selecionado.Name)
                {
                    case "Mood":
                    case "SleepQuality":
                        data = selecionado.Name == "Mood" 
                            ? minhaColecao.Select(r => r.Mood).ToArray() 
                            : minhaColecao.Select(r => r.SleepQuality).ToArray();
                        DataType = selecionado.Name == "Mood" ? "Humor" : "Qualidade do Sono";
                        Results.IsVisible = false;
                        break;
                    case "FoodCalories":
                    case "Duration":
                        data = selecionado.Name == "FoodCalories" 
                            ? minhaColecao.Select(r => r.FoodCalories).ToArray() 
                            : minhaColecao.Select(r => r.Duration).ToArray();
                        DataType = selecionado.Name == "FoodCalories" ? "Calorias (kCal)" : "Tempo de exercitação (min)";
                        
                        var nonNullData = data.Where(d => d.HasValue).Select(d => d.Value).ToArray();
                        AverageText.Text = $"{Math.Round(nonNullData.DefaultIfEmpty(0).Average(), 2)} {(selecionado.Name == "FoodCalories" ? "kCal" : "min")}";
                        TotalText.Text = $"{Math.Round(nonNullData.DefaultIfEmpty(0).Sum(), 2)} {(selecionado.Name == "FoodCalories" ? "kCal" : "min")}";
                        break;
                    default:
                        data = new double?[] { 0 };
                        break;
                }
            }



        }

        public void PlotCaloriesVsDays()
        {
            // Limpa o gráfico atual
            AvaPlot1.Plot.Clear();
            // Cria um novo gráfico
            Plot myPlot = AvaPlot1.Plot;

            // Adicione os dados ao gráfico
            var dates = minhaColecao.Select(r => DateTime.Parse(r.Date)).ToArray();
            
            // Adiciona os dados ao gráfico como barras
            myPlot.Add.Scatter(dates, data);

            // Define os rótulos do eixo X como as datas formatadas
            myPlot.Axes.DateTimeTicksBottom();

            // Customize plot style
            myPlot.YLabel($"{DataType}");
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
