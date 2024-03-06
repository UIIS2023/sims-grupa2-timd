using System.Windows;
using System.Windows.Input;

namespace SimsProject.WPF.ViewModel.Guest1
{
    public class HelpViewModel : ViewModelBase
    {
        private readonly Window _window;
        public HelpViewModel(Window window)
        {
            _window = window;
            GoBackCommand = new RelayCommand(GoBackClick);
        }
        public ICommand GoBackCommand { get; set; }
        public void GoBackClick(object obj)
        {
            _window.Close();
        }
    }
}
