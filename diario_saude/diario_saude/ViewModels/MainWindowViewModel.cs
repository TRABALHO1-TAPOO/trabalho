using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;
namespace diario_saude.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private bool _isPaneOpen = false;
        public bool IsPaneOpen
        {
            get => _isPaneOpen;
            set => this.RaiseAndSetIfChanged(ref _isPaneOpen, value);
        }

        public ReactiveCommand<Unit, Unit> TriggerPane { get; }

        private ViewModelBase _currentPage = new HomePageViewModel();
        public ViewModelBase CurrentPage
        {
            get => _currentPage;
            set => this.RaiseAndSetIfChanged(ref _currentPage, value);
        }

        private ObservableCollection<ListItemTemplate> _items = new()
        {
            new ListItemTemplate(typeof(HomePageViewModel), "home_regular"),
            new ListItemTemplate(typeof(DailyRecordPageViewModel), "note_add_regular"),
            new ListItemTemplate(typeof(RecordHistoryPageViewModel), "calendar_regular"),
            new ListItemTemplate(typeof(ReportAndChartsPageViewModel), "data_histogram_regular")
        };

        private ListItemTemplate? _selectedListItem;
        public ListItemTemplate SelectedListItem
        {
            get => _selectedListItem!;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedListItem, value);
                OnSelectedListItemChanged(value);
            }
        }

        public void OnSelectedListItemChanged(ListItemTemplate? value)
        {
            if (value is null) return;
            var instance = Activator.CreateInstance(value.ModelType);
            if (instance is null) return;
            CurrentPage = (ViewModelBase)instance;
        }

        public ObservableCollection<ListItemTemplate> Items
        {
            get => _items;
            set => this.RaiseAndSetIfChanged(ref _items, value);
        }

        public MainWindowViewModel()
        {
            TriggerPane = ReactiveCommand.Create(() =>
            {
                IsPaneOpen = !IsPaneOpen;
            });
        }
    }
}
public class ListItemTemplate : ReactiveObject
{
    public ListItemTemplate(Type type, string iconKey)
    {
        ModelType = type;
        Label = type.Name.Replace("PageViewModel", "");
        Application.Current!.TryFindResource(iconKey, out var icon);
        Icon = (StreamGeometry)icon!;
    }

    public ListItemTemplate(Type type)
    {
        ModelType = type;
        Label = type.Name.Replace("PageViewModel", "");
    }

    public string Label { get; }
    public Type ModelType { get; }
    public StreamGeometry? Icon { get; }
}