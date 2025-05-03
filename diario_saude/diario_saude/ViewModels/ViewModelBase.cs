using ReactiveUI;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia;
using System.Reactive;
using diario_saude.Services;

namespace diario_saude.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {
        public ReactiveCommand<Unit, Unit> ToggleThemeCommand { get; }

        private Brush _contentBackgroundColor = new SolidColorBrush(Color.Parse("#2d2d2d"));

        public Brush ContentBackgroundColor
        {
            get => _contentBackgroundColor;
            set => this.RaiseAndSetIfChanged(ref _contentBackgroundColor, value);
        }

        private string _themePreference = ThemeService.Preference;

        public string ThemePreference
        {
            get => _themePreference;
            set
            {
                this.RaiseAndSetIfChanged(ref _themePreference, value);
                ThemeService.Preference = value;
                SetTheme();
            }
        }

        public ViewModelBase()
        {
            ThemeService.PreferenceChanged += OnThemeChanged;
            SetTheme();
            ToggleThemeCommand = ReactiveCommand.Create(ToggleTheme);
        }

        private void OnThemeChanged(string newPreference)
        {
            ThemePreference = newPreference;
        }

        protected void SetTheme()
        {
            if (ThemePreference == "claro")
            {
                SetLightTheme();
            }
            else
            {
                SetDarkTheme();
            }
        }

        protected virtual void SetLightTheme()
        {
            ContentBackgroundColor = new SolidColorBrush(Color.Parse("#f2f2f2"));
            (Avalonia.Application.Current as Application)!.RequestedThemeVariant = ThemeVariant.Light;
        }

        protected virtual void SetDarkTheme()
        {
            ContentBackgroundColor = new SolidColorBrush(Color.Parse("#2d2d2d"));
            (Avalonia.Application.Current as Application)!.RequestedThemeVariant = ThemeVariant.Dark;
        }

        public void ToggleTheme()
        {
            ThemeService.Toggle();
        }
    }
}
