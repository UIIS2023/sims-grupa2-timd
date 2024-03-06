using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.WPF.ViewModel.Domain;

namespace SimsProject.WPF.ViewModel.Owner
{
    public class MainWindowViewModel : ViewModelBase
    {
        public User Owner { get; set; }
        public ObservableCollection<OwnerNotificationViewModel> Notifications { get; set; }
        public NavigationService NavigationService { get; set; }
        
        #region SERVICES

        private IOwnerNotificationService _notificationService;

        #endregion

        #region PROPERTIES

        private bool _isNotificationPopupOpen;
        public bool IsNotificationPopupOpen
        {
            get => _isNotificationPopupOpen;
            set
            {
                if (_isNotificationPopupOpen != value)
                {
                    _isNotificationPopupOpen = value;
                    OnPropertyChanged(nameof(IsNotificationPopupOpen));
                }
            }
        }

        #endregion

        #region COMMANDS

        public ICommand GoToNotificationCommand { get; set; }
        public ICommand OpenProfileCommand { get; set; }
        public ICommand OpenNotificationsCommand { get; set; }
        public ICommand OpenMainCommand { get; set; }
        public ICommand OpenAccommodationsCommand { get; set; }
        public ICommand OpenReservationsCommand { get; set; }
        public ICommand OpenReviewsCommand { get; set; }
        public ICommand OpenRenovationsCommand { get; set; }
        public ICommand OpenForumCommand { get; set; }
        public ICommand StartDemoCommand { get; set; }
        public ICommand LogoutCommand { get; set; }

        #endregion

        #region EVENTS

        public bool ConfirmActionResult { get; set; }
        public event EventHandler ConfirmActionRequested;
        public event EventHandler LogoutRequested;

        #endregion

        public MainWindowViewModel(User owner, NavigationService navigationService)
        {
            Owner = owner;
            NavigationService = navigationService;

            InitializeServices();
            InitializeCollections();
            InitializeCommands();
            InitializeNotifications();
        }

        private void InitializeServices()
        {
            _notificationService = Injector.CreateInstance<IOwnerNotificationService>();
        }

        private void InitializeCollections()
        {
            Notifications = new ObservableCollection<OwnerNotificationViewModel>();
        }

        private void InitializeCommands()
        {
            GoToNotificationCommand = new RelayCommand(GoToNotification);
            OpenProfileCommand = new RelayCommand(OpenProfile);
            OpenNotificationsCommand = new RelayCommand(OpenNotifications);
            OpenMainCommand = new RelayCommand(OpenMain);
            OpenAccommodationsCommand = new RelayCommand(OpenAccommodations);
            OpenReservationsCommand = new RelayCommand(OpenReservations);
            OpenReviewsCommand = new RelayCommand(OpenReviews);
            OpenRenovationsCommand = new RelayCommand(OpenRenovations);
            OpenForumCommand = new RelayCommand(OpenForum);
            StartDemoCommand = new RelayCommand(StartDemo);
            LogoutCommand = new RelayCommand(Logout);
        }

        public void InitializeNotifications()
        {
            foreach (var notification in _notificationService.GetInboxByUser(Owner))
            {
                Notifications.Add(new OwnerNotificationViewModel(_notificationService, notification));
            }
        }

        private void GoToNotification(object obj)
        {
            var selectedNotification = obj as OwnerNotificationViewModel;

            switch (selectedNotification?.Type)
            {
                case NotificationType.Cancel:
                    OpenReservations(null);
                    break;
                case NotificationType.Review:
                    OpenReviews(null);
                    break;
                case NotificationType.Reschedule:
                    OpenReservations(null);
                    break;
                case NotificationType.Forum:
                    OpenForum(null);
                    break;
            }

            ReadNotification(selectedNotification);
        }

        private void ReadNotification(OwnerNotificationViewModel selectedNotification)
        {
            _notificationService.ReadNotification(selectedNotification!.Id);
            Notifications.Remove(selectedNotification);
            IsNotificationPopupOpen = false;
        }

        private void OpenNotifications(object obj)
        {
            IsNotificationPopupOpen = true;
        }

        private void OpenProfile(object obj)
        {
            NavigationService.Navigate(new Uri("../../WPF/View/Owner/ProfilePanel.xaml", UriKind.Relative));
        }

        private void OpenMain(object obj)
        {
            NavigationService.Navigate(new Uri("../../WPF/View/Owner/MainPanel.xaml", UriKind.Relative));
        }

        private void OpenAccommodations(object obj)
        {
            NavigationService.Navigate(new Uri("../../WPF/View/Owner/AccommodationsPanel.xaml", UriKind.Relative));
        }

        private void OpenReservations(object obj)
        {
            NavigationService.Navigate(new Uri("../../WPF/View/Owner/ReservationsPanel.xaml", UriKind.Relative));
        }

        private void OpenReviews(object obj)
        {
            NavigationService.Navigate(new Uri("../../WPF/View/Owner/ReviewsPanel.xaml", UriKind.Relative));
        }

        private void OpenRenovations(object obj)
        {
            NavigationService.Navigate(new Uri("../../WPF/View/Owner/RenovationsPanel.xaml", UriKind.Relative));
        }

        private void OpenForum(object obj)
        {
            NavigationService.Navigate(new Uri("../../WPF/View/Owner/ForumPanel.xaml", UriKind.Relative));
        }

        private void StartDemo(object obj)
        {
            MessageBox.Show("Not implemented yet! :(");
        }

        private void Logout(object obj)
        {
            ConfirmActionRequested?.Invoke(this, EventArgs.Empty);
            if (ConfirmActionResult)
            {
                LogoutRequested?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}