using System.Windows;
using SimsProject.Domain.Model;
using System.Windows.Navigation;

namespace SimsProject.WPF.ViewModel
{
    public class AppViewModel : ViewModelBase
    {
        public NavigationService NavigationService { get; set; }

        public Window MainWindow { get; set; }

        private User _loggedInUser;
        public User LoggedInUser
        {
            get => _loggedInUser;
            set
            {
                if (_loggedInUser != value)
                {
                    _loggedInUser = value;
                    OnPropertyChanged(nameof(LoggedInUser));
                }
            }
        }

        private ResourceDictionary _theme;
        public ResourceDictionary Theme
        {
            get => _theme;
            set
            {
                if (_theme != value)
                {
                    _theme = value;
                    OnPropertyChanged(nameof(Theme));
                }
            }
        }
    }
}
