using Avalonia;
using Avalonia.Media;
using Avalonia.Styling;
using diario_saude.Services;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;

namespace diario_saude.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private Brush _menuBackgroundColor = new SolidColorBrush(Color.Parse("#1e1e1e"));

        public Brush MenuBackgroundColor
        {
            get => _menuBackgroundColor;
            set => this.RaiseAndSetIfChanged(ref _menuBackgroundColor, value);
        }

        private bool _isPaneOpen = false;

        public bool IsPaneOpen
        {
            get => _isPaneOpen;
            set => this.RaiseAndSetIfChanged(ref _isPaneOpen, value);
        }

        public ReactiveCommand<Unit, Unit> TriggerPane { get; }
        public ReactiveCommand<Unit, Unit> ToggleThemeCommand { get; }

        private ViewModelBase _currentPage;

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
            new ListItemTemplate(typeof(ReportAndChartsPageViewModel), "data_histogram_regular"),
            new ListItemTemplate(typeof(SettingsPageViewModel), "settings_regular"),
        };

        public ObservableCollection<ListItemTemplate> Items
        {
            get => _items;
            set => this.RaiseAndSetIfChanged(ref _items, value);
        }

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

        public MainWindowViewModel()
        {
            SetTheme(); // Aplicar o tema ao inicializar

            TriggerPane = ReactiveCommand.Create(() =>
            {
                IsPaneOpen = !IsPaneOpen;
            });

            ToggleThemeCommand = ReactiveCommand.Create(() =>
            {
                ThemeService.Toggle();
            });

            // Atualizar a interface quando o tema mudar
            ThemeService.PreferenceChanged += OnThemeChanged;

            CurrentPage = new HomePageViewModel(this);
        }

        private void OnThemeChanged(string newTheme)
        {
            ThemePreference = newTheme; // Atualizar a propriedade ThemePreference
            SetTheme(); // Aplicar o tema atualizado
            Console.WriteLine($"Tema alterado para: {newTheme}");
        }

        public void OnSelectedListItemChanged(ListItemTemplate? value)
        {
            if (value is null) return;
            object? instance = null;

            if (value.ModelType == typeof(HomePageViewModel))
            {
                instance = new HomePageViewModel(this);
            }
            else
            {
                instance = Activator.CreateInstance(value.ModelType);
            }

            if (instance is ViewModelBase vm)
            {
                CurrentPage = vm;
            }
        }

        public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
        {
            var item = Items.FirstOrDefault(i => i.ModelType == typeof(TViewModel));
            if (item != null)
            {
                SelectedListItem = item;
            }
        }

        protected override void SetDarkTheme()
        {
            ContentBackgroundColor = new SolidColorBrush(Color.Parse("#2d2d2d"));
            MenuBackgroundColor = new SolidColorBrush(Color.Parse("#1e1e1e"));
            (Avalonia.Application.Current as Application)!.RequestedThemeVariant = ThemeVariant.Dark;
        }

        protected override void SetLightTheme()
        {
            ContentBackgroundColor = new SolidColorBrush(Color.Parse("#e2e2e2"));
            MenuBackgroundColor = new SolidColorBrush(Color.Parse("#f3f3f3"));
            (Avalonia.Application.Current as Application)!.RequestedThemeVariant = ThemeVariant.Light;
        }
    }
}
