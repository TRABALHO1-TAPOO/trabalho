// HomePageViewModel.cs
using diario_saude.ViewModels;
using ReactiveUI;
using System.Reactive;

namespace diario_saude.ViewModels
{
    public class HomePageViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public ReactiveCommand<Unit, Unit> NavigateToDailyRecordCommand { get; }
        public ReactiveCommand<Unit, Unit> NavigateToRecordHistoryCommand { get; }

        public HomePageViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;

            NavigateToDailyRecordCommand = ReactiveCommand.Create(() =>
            {
                _mainWindowViewModel.NavigateTo<DailyRecordPageViewModel>();
            });

            NavigateToRecordHistoryCommand = ReactiveCommand.Create(() =>
            {
                _mainWindowViewModel.NavigateTo<RecordHistoryPageViewModel>();
            });
        }
    }
}