using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using SimsProject.Domain.Model;
using SimsProject.Repository;
using SimsProject.WPF.ViewModel.Guest2;

namespace SimsProject.WPF.View.Guest2
{
    /// <summary>
    /// Interaction logic for GuideOverview.xaml
    /// </summary>
    public partial class Guest2Overview : Window
    {
        /*
        public static ObservableCollection<Tour> Tours { get; set; }
        public List<Tour> AllTours { get; set; }

        public List<TourReservation> TourReservations { get; set; }
        public List<TourAttendance> TourAttendances { get; set; }
        public Tour SelectedTour { get; set; }
        public User LoggedInUser { get; set; }


        private readonly TourReservationCsvRepository _tourReservationRepository;
        private readonly LocationCsvRepository _locationRepository;
        private readonly CheckPointCsvRepository _checkPointRepository;
        private readonly TourDateCsvRepository _tourDateRepository;
        private readonly TourImageCsvRepository _imageRepository;
        private readonly UserCsvRepository _userRepository;
        private readonly TourCsvRepository _tourRepository;
        private readonly TourAttendanceRepository _tourAttendanceRepository;

        */
        public Guest2Overview(User user)
        {
            InitializeComponent();
            this.DataContext = new Guest2OverviewViewModel(user,this);
           // LoggedInUser = user;

            /*
            _tourReservationRepository = new TourReservationCsvRepository();
            _locationRepository = new LocationCsvRepository();
            _checkPointRepository = new CheckPointCsvRepository();
            _tourDateRepository = new TourDateCsvRepository();
            _userRepository = new UserCsvRepository();
            _imageRepository = new TourImageCsvRepository();
            _tourRepository = new TourCsvRepository();
            _tourAttendanceRepository = new TourAttendanceRepository();
            TourAttendances = new List<TourAttendance>(_tourAttendanceRepository.GetAll());

            TourReservations = new List<TourReservation>(_tourReservationRepository.GetAll());
            AllTours = new List<Tour>(_tourRepository.GetAll());
            Tours = new ObservableCollection<Tour>(_tourReservationRepository.GetReservedToursByUser(user, AllTours));
            PopulateTours();
            PopulateTourAttendances();
            GetSortedCheckPoints();
            */
            

        }
        /*
        private void GetNotifications()
        {
            foreach (var attendance in TourAttendances)
            {
                if (attendance.User.Id.Equals(LoggedInUser.Id) && attendance.Present == Presence.GuideConfirmed)
                {
                    MessageBoxResult result = MessageBox.Show($"Are you present on {attendance.Tour.Name}?", "Confirm your presence", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        attendance.Present = Presence.GuestConfirmed;
                        _tourAttendanceRepository.Update(attendance);
                    }
                    else
                    {
                        attendance.Present = Presence.GuestNotPresent;
                        _tourAttendanceRepository.Update(attendance);
                    }
                }
            }
        }

        private void PopulateTourAttendances()
        {
            foreach (var attendance in TourAttendances)
            {
                foreach (var tour in Tours)
                {
                    if (tour.Id == attendance.Tour.Id)
                    {
                        attendance.Tour.Name = tour.Name;
                    }
                }
            }
        }

        private void PopulateTours()
        {
            foreach (var tour in AllTours)
            {
                tour.TourLocation = _locationRepository.GetById(tour.TourLocation.Id);
                tour.CheckPoints = _checkPointRepository.GetByParentId(tour.Id);
                tour.User = _userRepository.GetById(tour.User.Id);
                tour.TourDates = _tourDateRepository.GetByParentId(tour.Id);
                tour.Images = _imageRepository.GetByParentId(tour.Id);
                tour.Cover = tour.Images[0];
            }
        }

        public void GetSortedCheckPoints()
        {
            foreach (var tour in Tours)
            {
                SortCheckPoints(tour.CheckPoints);
            }
        }

        public void SortCheckPoints(List<CheckPoint> checkPoints)
        {
            CheckPoint lastCheckPoint = checkPoints[1];
            checkPoints.RemoveAt(1);
            checkPoints.Add(lastCheckPoint);
        }


        private void Logout(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = ConfirmLogout();
            if (result == MessageBoxResult.Yes)
            {
                System.Windows.Application.Current.Windows.OfType<Window>()
                    .Where(w => w != this)
                    .ToList()
                    .ForEach(w => w.Close());

                LogInForm logInForm = new LogInForm();
                logInForm.Show();

                Close();
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

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            SearchTourWindow searchTourWindow = new SearchTourWindow(LoggedInUser);
            searchTourWindow.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                GetNotifications();
            }));
        }
        */
    }
}