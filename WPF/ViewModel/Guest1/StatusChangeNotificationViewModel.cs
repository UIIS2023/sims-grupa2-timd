using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using System;
using SimsProject.WPF.View.Guest2;

namespace SimsProject.WPF.ViewModel.Guest1
{
    public class StatusChangeNotificationViewModel
    {
        public NavigationService NavigationService { get; set; }
        private readonly Window _window;
        public StatusChangeNotificationViewModel(Window window, NavigationService navigationService)
        {
            _window = window;
            ViewStatusCommand = new RelayCommand(ViewStatusClick);
            GoBackCommand = new RelayCommand(GoBackClick);
        }
        public ICommand GoBackCommand { get; set; }
        public void GoBackClick(object obj)
        {
            _window.Close();
        }
        public ICommand ViewStatusCommand { get; set; }
        public void ViewStatusClick(object obj)
        {
            _window.Close();
            //NavigationService.Navigate(
              //  new Uri("/SimsProject;component/WPF/View/Guest1/StatusOfMoveRequestsForm.xaml", UriKind.Relative));
        }
    }
}
