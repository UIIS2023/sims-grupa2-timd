using SimsProject.Domain.Model;
using SimsProject.WPF.View.Guest1;
using System.Windows;
using System.Windows.Input;
using SimsProject.Application.Interface;
using SimsProject.Application.Localization;
using System;
using System.Windows.Navigation;
using SimsProject.Localization;
using System.Windows.Threading;

namespace SimsProject.WPF.ViewModel.Guest1
{
    public class Guest1OverviewViewModel : ViewModelBase
    {
        public User CurrentUser { get; set; }
        public NavigationService NavigationService { get; set; }
        private string language = "en";
        private readonly App app;
        private readonly IAccommodationReservationRescheduleService _accommodationReservationRescheduleService;
        private readonly IGuest1Service _guest1Service;
        public event EventHandler LogoutRequested;
        public Guest1OverviewViewModel(User currentUser, NavigationService navigationService)
        {
            NavigationService = navigationService;
            app = (App)System.Windows.Application.Current;
            app.ChangeLanguage("en-US");
            IsDarkMode = true;
            IsLightMode = false;
            _guest1Service = Injector.CreateInstance<IGuest1Service>();
            _accommodationReservationRescheduleService = Injector.CreateInstance<IAccommodationReservationRescheduleService>();
            CurrentUser = _guest1Service.CheckSuperGuest(currentUser);
            ProfileCommand = new RelayCommand(ProfileClick);
            SearchPageCommand = new RelayCommand(SearchPageClick);
            YourReservationsCommand = new RelayCommand(YourReservationsClick);
            ReviewCommand = new RelayCommand(OpenReview);
            AnywhereAnytimeCommand = new RelayCommand(AnywhereAnytimeClick);
            ForumsCommand = new RelayCommand(ForumsClick);
            HelpCommand = new RelayCommand(HelpClick);
            ChangeThemeCommand = new RelayCommand(ChangeThemeClick);
            ChangeLanguageCommand = new RelayCommand(ChangeLanguageClick);
            LogoutCommand = new RelayCommand(Logout);
        }
        
        public ICommand ProfileCommand { get; set; }
        public void ProfileClick(object obj)
        {
            NavigationService.Navigate(
                new Uri("/SimsProject;component/WPF/View/Guest1/ProfilePage.xaml", UriKind.Relative));
        }
        public ICommand SearchPageCommand { get; set; }
        public void SearchPageClick(object obj)
        {
            NavigationService.Navigate(
                new Uri("/SimsProject;component/WPF/View/Guest1/SearchPage.xaml", UriKind.Relative));
        }
        public ICommand YourReservationsCommand { get; set; }
        public void YourReservationsClick(object obj)
        {
            NavigationService.Navigate(
                new Uri("/SimsProject;component/WPF/View/Guest1/YourReservationsForm.xaml", UriKind.Relative));
        }
        public ICommand ReviewCommand { get; set; }
        private void OpenReview(object obj)
        {
            NavigationService.Navigate(
                new Uri("/SimsProject;component/WPF/View/Guest1/ReviewsForm.xaml", UriKind.Relative));
        }
        public ICommand AnywhereAnytimeCommand { get; set; }
        public void AnywhereAnytimeClick(object obj)
        {
            NavigationService.Navigate(
                new Uri("/SimsProject;component/WPF/View/Guest1/AnywhereAnytimeForm.xaml", UriKind.Relative));
        }
        public ICommand ForumsCommand { get; set; }
        public void ForumsClick(object obj)
        {
            NavigationService.Navigate(
                new Uri("/SimsProject;component/WPF/View/Guest1/ForumsForm.xaml", UriKind.Relative));
        }
        public ICommand HelpCommand { get; set; }
        public void HelpClick(object obj)
        {
            HelpWindow form = new();
            form.ShowDialog();
        }
        public ICommand ChangeThemeCommand { get; set; }
        public void ChangeThemeClick(object obj)
        {
            AppTheme.ChangeTheme(IsDarkMode
                ? new Uri("Resources/Styles/Guest1StyleLight.xaml", UriKind.Relative)
                : new Uri("Resources/Styles/Guest1StyleDark.xaml", UriKind.Relative));
            IsDarkMode = !IsDarkMode;
            IsLightMode = !IsLightMode;
        }
        public ICommand ChangeLanguageCommand { get; set; }
        public void ChangeLanguageClick(object obj)
        {
            if (language == "en")
            {
                app.ChangeLanguage("sr-Cyrl-RS");
                language = "sr";
            }
            else
            {
                app.ChangeLanguage("en-US");
                language = "en";
            }
        }
        public ICommand LogoutCommand { get; set; }
        private void Logout(object obj)
        {
            if (ConfirmLogout() == MessageBoxResult.Yes)
            {
                LogoutRequested?.Invoke(this, EventArgs.Empty);
            }
        }
        private static MessageBoxResult ConfirmLogout()
        {
            string sMessageBoxText = TranslationSource.Instance["AYL"];
            string sCaption = TranslationSource.Instance["Logout"]; 
            MessageBoxButton btnMessageBox = MessageBoxButton.YesNo;
            MessageBoxImage icnMessageBox = MessageBoxImage.Question;
            MessageBoxResult result = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
            return result;
        }

        private static MessageBoxResult ConfirmNotification()
        {
            string sMessageBoxText = "Your request status changed. Do you want to see it?";
            string sCaption = "Notification";
            MessageBoxButton btnMessageBox = MessageBoxButton.YesNo;
            MessageBoxImage icnMessageBox = MessageBoxImage.Question;
            MessageBoxResult result = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
            return result;
        }

        private bool _isDarkMode;
        public bool IsDarkMode
        {
            get => _isDarkMode;
            set
            {
                _isDarkMode = value;
                OnPropertyChanged(nameof(IsDarkMode));
            }
        }
        
        private bool _isLightMode;
        public bool IsLightMode
        {
            get => _isLightMode;
            set
            {
                _isLightMode = value;
                OnPropertyChanged(nameof(IsLightMode));
            }
        }
        public void Notify()
        {
            if (_accommodationReservationRescheduleService.ShouldNotify(CurrentUser))
            {
                //StatusChangeNotification form = new();
                //form.Show();
                if (ConfirmNotification() == MessageBoxResult.Yes)
                {
                    NavigationService.Navigate(
                        new Uri("/SimsProject;component/WPF/View/Guest1/StatusOfMoveRequestsForm.xaml", UriKind.Relative));
                }
            }
        }
    }
}
