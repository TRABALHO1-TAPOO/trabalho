using ReactiveUI;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia;

namespace diario_saude.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {

        private Brush _contentBackgroundColor = new SolidColorBrush(Color.Parse("#2d2d2d"));
        public Brush ContentBackgroundColor
        {
            get => _contentBackgroundColor;
            set => this.RaiseAndSetIfChanged(ref _contentBackgroundColor, value);
        }

        private string _themePreference;

        public string ThemePreference
        {
            get => _themePreference;
            set
            {
                this.RaiseAndSetIfChanged(ref _themePreference, value);
                SetBackgroundColorBasedOnTheme();
            } 
        }

        public ViewModelBase()
        {
            ThemePreference = "Light";
            SetBackgroundColorBasedOnTheme();
        }

        private void SetBackgroundColorBasedOnTheme()
        {
            if(ThemePreference == "Light")
            {
                ContentBackgroundColor = new SolidColorBrush(Color.Parse("#f2f2f2"));
                (Avalonia.Application.Current as Application)!.RequestedThemeVariant = ThemeVariant.Light;
            }
            else 
            {
                ContentBackgroundColor = new SolidColorBrush(Color.Parse("#2d2d2d"));
                (Avalonia.Application.Current as Application)!.RequestedThemeVariant = ThemeVariant.Dark;

            }
        }

        public void ToggleTheme()
        {
            ThemePreference = ThemePreference == "Light" ? "Dark" : "Light";
        }

    }
}
