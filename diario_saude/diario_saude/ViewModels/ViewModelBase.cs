using ReactiveUI;
using Avalonia.Media;

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

        public ViewModelBase()
        {
        }

    }
}
