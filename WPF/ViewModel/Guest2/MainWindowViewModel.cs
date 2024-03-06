using SimsProject.Domain.Model;
using SimsProject.WPF.View.Authentication;
using SimsProject.WPF.View.Guest2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SimsProject.WPF.ViewModel.Guest2
{
    public class MainWindowViewModel 
    {
       public User LoggedInUser { get; set; }
       public MainWindow _mainWindow;
       public RelayCommand RelayCommandWithParams { get; set; }
        public ICommand NavigationButtonCommand => new RelayCommandWithParams(Navigations);

        private ICommand _logoutCommand;
        public ICommand LogoutCommand
        {
            get
            {
                if (_logoutCommand == null)
                {
                    _logoutCommand = new RelayCommand(
                        param => this.Logout(),
                        param => this.CanLogout()
                    );
                }
                return _logoutCommand;
            }
        }
        public bool CanLogout()
        {
            return true;
        }
        public MainWindowViewModel(User currentUser,MainWindow mainWindow) {
            LoggedInUser = currentUser;
            _mainWindow= mainWindow;


        }

        public void Navigations(object param) {
            string nextPage = param.ToString();
            var navigationService = _mainWindow.MainFrame.NavigationService;

            switch (nextPage)
            {
                case "ExploreTours":
                    navigationService.Navigate(new Explore(LoggedInUser));
                    break;
                case "MyReviews":
                    navigationService.Navigate(new MyReviews(LoggedInUser));
                    break;
                case "Vouchers":
                    navigationService.Navigate(new MyVouchers(LoggedInUser));
                    break;
                case "MyRequests":
                    navigationService.Navigate(new MyRequests(LoggedInUser));
                    break;
                case "MyComplexRequests":
                    navigationService.Navigate(new MyComplexRequests(LoggedInUser));
                    break;
                case "MyReservations":
                    navigationService.Navigate(new MyReservations(LoggedInUser));
                    break;


            }
        
        }

        private void Logout()
        {
            MessageBoxResult result = ConfirmLogout();
            if (result == MessageBoxResult.Yes)
            {
                System.Windows.Application.Current.Windows.OfType<Window>()
                    .Where(w => w != _mainWindow)
                    .ToList()
                    .ForEach(w => w.Close());

                LogInForm logInForm = new LogInForm();
                logInForm.Show();

                _mainWindow.Close();
            }
        }

        private static MessageBoxResult ConfirmLogout()
        {
            string sMessageBoxText = "Are you sure you want to logout?";
            string sCaption = "Logout";

            MessageBoxButton btnMessageBox = MessageBoxButton.YesNo;
            MessageBoxImage icnMessageBox = MessageBoxImage.Question;

            MessageBoxResult result = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
            return result;
        }

    }
}
