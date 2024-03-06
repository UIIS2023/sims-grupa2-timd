using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Navigation;
using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.WPF.ViewModel.Domain;

namespace SimsProject.WPF.ViewModel.Owner
{
    public class ProfilePanelViewModel : ViewModelBase
    {
        public User Owner { get; set; }
        public NavigationService NavigationService { get; set; }
        public ObservableCollection<OwnerNotificationViewModel> Notifications { get; set; }

        #region SERVICES

        private IOwnerNotificationService _notificationService;
        private IOwnerService _statisticsService;
        private IUserService _userService;

        #endregion

        #region PROPERTIES

        public string CurrentPassword { private get; set; }
        public string NewPassword { private get; set; }

        private double _averageRating;
        public double AverageRating
        {
            get => _averageRating;
            set
            {
                if (_averageRating != value)
                {
                    _averageRating = value;
                    OnPropertyChanged(nameof(AverageRating));
                }
            }
        }

        private int _reviewCount;
        public int ReviewCount
        {
            get => _reviewCount;
            set
            {
                if (_reviewCount != value)
                {
                    _reviewCount = value;
                    OnPropertyChanged(nameof(ReviewCount));
                }
            }
        }

        private int _accommodationCount;
        public int AccommodationCount
        {
            get => _accommodationCount;
            set
            {
                if (_accommodationCount != value)
                {
                    _accommodationCount = value;
                    OnPropertyChanged(nameof(AccommodationCount));
                }
            }
        }

        private int _pastReservationCount;
        public int PastReservationCount
        {
            get => _pastReservationCount;
            set
            {
                if (_pastReservationCount != value)
                {
                    _pastReservationCount = value;
                    OnPropertyChanged(nameof(PastReservationCount));
                }
            }
        }

        private bool _isSuperOwner;
        public bool IsSuperOwner
        {
            get => _isSuperOwner;
            set
            {
                if (_isSuperOwner != value)
                {
                    _isSuperOwner = value;
                    OnPropertyChanged(nameof(IsSuperOwner));
                }
            }
        }

        #endregion

        #region COMMANDS

        public ICommand ChangePasswordCommand { get; set; }
        public ICommand OpenSuperOwnerCommand { get; set; }
        public ICommand GoToNotificationCommand { get; set; }

        #endregion

        #region EVENTS

        public event EventHandler SuperOwnerRequested;
        public event EventHandler SuccessRequested;
        public event EventHandler FailRequested;

        #endregion

        public ProfilePanelViewModel(User owner, NavigationService navigationService)
        {
            Owner = owner;
            NavigationService = navigationService;

            InitializeServices();
            InitializeCollections();
            InitializeCommands();
            InitializeNotifications();
            InitializeStatistics();
        }

        private void InitializeServices()
        {
            _notificationService = Injector.CreateInstance<IOwnerNotificationService>();
            _statisticsService = Injector.CreateInstance<IOwnerService>();
            _userService = Injector.CreateInstance<IUserService>();
        }

        private void InitializeCollections()
        {
            Notifications = new ObservableCollection<OwnerNotificationViewModel>();
        }

        private void InitializeCommands()
        {
            ChangePasswordCommand = new RelayCommand(ChangePassword, CanChangePassword);
            OpenSuperOwnerCommand = new RelayCommand(OpenSuperOwner);
            GoToNotificationCommand = new RelayCommand(GoToNotification);
        }

        public void InitializeNotifications()
        {
            foreach (var notification in _notificationService.GetByUser(Owner))
            {
                Notifications.Add(new OwnerNotificationViewModel(_notificationService, notification));
            }
        }

        public void InitializeStatistics()
        {
            AverageRating = Math.Round(_statisticsService.GetAverageRating(Owner), 2);
            ReviewCount = _statisticsService.GetReviewCount(Owner);
            AccommodationCount = _statisticsService.GetAccommodationCount(Owner);
            PastReservationCount = _statisticsService.GetPastReservationCount(Owner);
            IsSuperOwner = Owner.IsSuperUser;
        }

        private void ChangePassword(object obj)
        {
            if (_userService.ChangePassword(Owner, CurrentPassword, NewPassword))
            {
                SuccessRequested?.Invoke(this, EventArgs.Empty);
                return;
            }

            FailRequested?.Invoke(this, EventArgs.Empty);
        }

        private bool CanChangePassword(object obj)
        {
            return !string.IsNullOrEmpty(CurrentPassword) && !string.IsNullOrEmpty(NewPassword);
        }

        private void OpenSuperOwner(object obj)
        {
            SuperOwnerRequested?.Invoke(this, EventArgs.Empty);
        }

        private void GoToNotification(object obj)
        {
            var selectedNotification = obj as OwnerNotificationViewModel;

            switch (selectedNotification?.Type)
            {
                case NotificationType.Cancel:
                    NavigationService.Navigate(
                        new Uri("../../WPF/View/Owner/ReservationsPanel.xaml", UriKind.Relative)); 
                    break;
                case NotificationType.Review:
                    NavigationService.Navigate(
                        new Uri("../../WPF/View/Owner/ReviewsPanel.xaml", UriKind.Relative)); 
                    break;
                case NotificationType.Reschedule:
                    NavigationService.Navigate(
                        new Uri("../../WPF/View/Owner/ReservationsPanel.xaml", UriKind.Relative));
                    break;
                case NotificationType.Forum:
                    NavigationService.Navigate(
                        new Uri("../../WPF/View/Owner/ForumPanel.xaml", UriKind.Relative));
                    break;
            }
        }
    }
}