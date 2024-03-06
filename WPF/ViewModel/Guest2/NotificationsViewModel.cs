using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using SimsProject.Application.UseCase;
using SimsProject.Domain.Model;
using SimsProject.WPF.View.Guest2;

namespace SimsProject.WPF.ViewModel.Guest2
{
    public class NotificationsViewModel : ViewModelBase
    {
        public User LoggedInUser;
        public Tour CreatedTour;
      
        public static ObservableCollection<Notification> Notifications { get; set; }

        public Guest2NotificationService _guest2NotificationService;
        public TourService _tourService;

        private Notification _selectedNotification;
        public Notification SelectedNotification
        {
            get { return _selectedNotification; }
            set
            {
                _selectedNotification = value;
                OnPropertyChanged(nameof(SelectedNotification));
                Read();
            }
        }
        public NotificationsViewModel(User currentUser) {
            LoggedInUser = currentUser;
            _guest2NotificationService = new();
            Notifications = new();
            CreatedTour = new();
            _tourService = new();
            UpdateTable();
        }

        public void UpdateTable()
        {
            Notifications.Clear();
            foreach (Notification notification in _guest2NotificationService.GetByUser(LoggedInUser)) {
                if (notification.IsRead==false) {
                    Notifications.Add(notification);
                }
            }

        }
        private MessageBoxResult ShowNotification()
        {
            string sMessageBoxText = SelectedNotification.Message;
            string sCaption = "Message";
            MessageBoxButton btnMessageBox = MessageBoxButton.OK;
            return MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox);
        }
        public void Read() {
            MessageBoxResult result = ShowNotification();
            if (result == MessageBoxResult.OK) {
                _guest2NotificationService.ReadNotification(SelectedNotification.Id);
                UpdateTable();
                CreatedTour = _tourService.GetById(FindTourId(SelectedNotification));
                ShowTour(CreatedTour);
            }

        }

        public int FindTourId(Notification selectedNotification) {

            string input = selectedNotification.Message;
            string pattern = @"TourID (\d+)";
            Match match = Regex.Match(input, pattern);
            int id=-1;
            if (match.Success)
            {
                 id = int.Parse(match.Groups[1].Value);
               
            }

            return id;
        }

        public void ShowTour(Tour tour) {
            var window = new AcceptedRequestInfo(tour);
            window.Show();
        }
    }
}
