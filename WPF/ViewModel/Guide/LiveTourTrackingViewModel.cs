using SimsProject.Domain.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SimsProject.WPF.ViewModel.Domain;
using SimsProject.Application.Interface;
using System.Windows.Input;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SimsProject.WPF.View.Guide;

namespace SimsProject.WPF.ViewModel.Guide
{
    public class LiveTourTrackingViewModel : ViewModelBase
    {
        #region PROPERTIES

        public ObservableCollection<CheckPointViewModel> CheckPoints { get; set; }
        public ObservableCollection<TourViewModel> Tours { get; set; }
        public ObservableCollection<TourViewModel> ToursToday { get; set; }
        public ObservableCollection<User> Guests { get; set; }
        public List<TourReservationViewModel> TourReservations { get; set; }
        public TourAttendance SelectedTourAttendance { get; set; }
        public TourViewModel SelectedTour { get; set; }

        private CheckPoint _checkPoint;
        public CheckPoint CheckPoint
        {
            get => _checkPoint;
            set
            {
                _checkPoint = value;
                OnPropertyChanged(nameof(CheckPoint));
            }
        }

        private ObservableCollection<TourAttendanceViewModel> _tourAttendances;
        public ObservableCollection<TourAttendanceViewModel> TourAttendances
        {
            get => _tourAttendances;
            set
            {
                _tourAttendances = value;
                OnPropertyChanged(nameof(TourAttendances));
            }
        }

        private CheckPoint _selectedCheckPoint;
        public CheckPoint SelectedCheckPoint
        {
            get => _selectedCheckPoint;
            set
            {
                if (value != _selectedCheckPoint)
                {
                    _selectedCheckPoint = value;
                    OnPropertyChanged(nameof(SelectedCheckPoint));
                }
            }
        }

        private bool _hasTourStarted;
        public bool HasTourStarted
        {
            get => _hasTourStarted;
            set
            {
                _hasTourStarted = value;
                OnPropertyChanged(nameof(HasTourStarted));
            }
        }

        private User LoggedInUser { get; }

        #endregion

        #region SERVICES

        private ITourService _tourService;
        private ITourReservationService _tourReservationService;
        private ICheckPointService _checkPointService;
        private ITourAttendanceService _tourAttendanceService;
        private ITourDateService _tourDateService;

        #endregion

        #region COMMANDS
        public ICommand MarkCheckPointCommand { get; set; }
        public ICommand FollowTourLiveCommand { get; set; }
        public ICommand EndTourPrematurelyCommand { get; set; }
        public ICommand RecordAttendanceCommand { get; set; }
        public ICommand OpenGuideOverviewCommand { get; set; }

        #endregion

        public LiveTourTrackingViewModel(User user)
        {
            LoggedInUser = user;

            InitializeServices();
            InitializeCollections();
            InitializeCommands();
            GetToursToday();
            LoadTourData();
        }

        private void InitializeServices()
        {
            _tourReservationService = Injector.CreateInstance<ITourReservationService>();
            _tourAttendanceService = Injector.CreateInstance<ITourAttendanceService>();
            _checkPointService = Injector.CreateInstance<ICheckPointService>();
            _tourDateService = Injector.CreateInstance<ITourDateService>();
            _tourService = Injector.CreateInstance<ITourService>();
        }

        private void InitializeCollections()
        {
            TourAttendances = new ObservableCollection<TourAttendanceViewModel>();
            TourReservations = new List<TourReservationViewModel>();
            CheckPoints = new ObservableCollection<CheckPointViewModel>();
            ToursToday = new ObservableCollection<TourViewModel>();
            Guests = new ObservableCollection<User>();
            Tours = new ObservableCollection<TourViewModel>();

            foreach (var tourReservation in _tourReservationService.GetByGuide(LoggedInUser))
            {
                TourReservations.Add(new TourReservationViewModel(tourReservation));
            }

            foreach (var tour in _tourService.GetAll())
            {
                Tours.Add(new TourViewModel(tour));
            }

            HasTourStarted = false;
        }

        private void InitializeCommands()
        {
            EndTourPrematurelyCommand = new RelayCommand(EndTourPrematurely);
            MarkCheckPointCommand = new RelayCommand(MarkCurrentCheckPoint);
            OpenGuideOverviewCommand = new RelayCommand(OpenGuideOverview);
            RecordAttendanceCommand = new RelayCommand(RecordAttendance);
            FollowTourLiveCommand = new RelayCommand(FollowTourLive);
        }

        public void GetToursToday()
        {
            foreach (var tour in _tourService.GetToursToday())
            {
                ToursToday.Add(new TourViewModel(tour));
            }
        }

        public void LoadTourData()
        {
            foreach (var tour in Tours)
            {
                foreach (var checkPoint in tour.CheckPointsViewModel.CheckPoints)
                {
                    if (!checkPoint.IsActive) continue;
                    SelectedTour = tour;
                    HasTourStarted = true;
                    GetCheckPoints();
                    RemoveTracklessTour(SelectedTour);
                    GetGuests();
                    GetTourAttendances();
                    PopulateTourAttendances();
                }
            }
        }

        public void GetCheckPoints()
        {
            foreach (var checkPoint in SelectedTour.CheckPointsViewModel.CheckPoints)
            {
                CheckPoints.Add(checkPoint);
            }
            SortCheckPoints(SelectedTour);
        }

        public void SortCheckPoints(TourViewModel selectedTour)
        {
            var secondCheckPoint = selectedTour.CheckPointsViewModel.CheckPoints[1];
            CheckPoints.RemoveAt(1);
            CheckPoints.Add(secondCheckPoint);
            CheckPoints[0].IsActive = true;

            _checkPointService.UpdateByTourId(CheckPoints[0].Id, selectedTour.Id);
        }

        public void RemoveTracklessTour(TourViewModel selectedTour)
        {
            foreach (var tour in ToursToday.ToList().Where(tour => tour.Id != selectedTour.Id))
            {
                ToursToday.Remove(tour);
            }
        }

        private void GetGuests()
        {
            var tourDate = SelectedTour.TourDatesViewModel.TourDates.Find(t => t.Date.Date == DateTime.Today);
            Guests = new ObservableCollection<User>(_tourReservationService.GetUsersByTourAndDate(SelectedTour.Id, tourDate.Date));
        }

        private void GetTourAttendances()
        {
            foreach (var guest in Guests)
            {
                TourAttendances.Add(new TourAttendanceViewModel(_tourAttendanceService.GetByGuest(guest)));
            }
        }

        private void FollowTourLive(object obj)
        {
            if (SelectedTour == null)
            {
                MessageBox.Show("Tour is not selected", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (HasTourStarted)
            {
                MessageBox.Show("Tour has already started", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                HasTourStarted = true;
                GetGuests();
                GetCheckPoints();
                RemoveTracklessTour(SelectedTour);
                SaveTourAttendances();
                PopulateTourAttendances();
            }
        }

        private void PopulateTourAttendances()
        {
            if (!HasTourStarted) return;
            foreach (var attendance in TourAttendances)
            {
                if (!attendance.Present.Equals(Presence.GuestNotPresent)) continue;
                attendance.CheckPoint.Name = "Guest did not show up";
                attendance.CheckPoint.Id = -1;
            }
        }

        private void SaveTourAttendances()
        {
            foreach (var guest in Guests)
            {
                var tour = _tourService.GetById(SelectedTour.Id);
                var newTourAttendance = new TourAttendance(tour, guest, Presence.NotPresent, new CheckPoint());
                var savedAttendance = _tourAttendanceService.Save(newTourAttendance);
                TourAttendances.Add(new TourAttendanceViewModel(savedAttendance));
            }
        }

        private void EndTourPrematurely(object obj)
        {
            if (HasTourStarted)
            {
                var result = ConfirmEndTour();
                if (result != MessageBoxResult.Yes) return;
                foreach (var tourDate in SelectedTour.TourDatesViewModel.TourDates)
                {
                    var dateOnly = GetSystemDateOnly();
                    var tourDateOnly = ExtractTourDate(tourDate);

                    if (!tourDateOnly.Equals(dateOnly) || tourDate.TourId != SelectedTour.Id) continue;
                    tourDate.HasEnded = true;
                    _tourDateService.UpdateById(tourDate.Id);
                }

                _checkPointService.SetDefault();
                OnActionCompleted();
            }
            else
            {
                MessageBox.Show("No tours have started yet", "End tour prematurely", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RecordAttendance(object obj)
        {
            if (SelectedTourAttendance == null)
            {
                MessageBox.Show("Please choose a guest");
            }
            else
            {
                if (SelectedTourAttendance.Present == Presence.NotPresent)
                {
                    SelectedTourAttendance.Present = Presence.GuideConfirmed;
                    SelectedTourAttendance.CheckPoint = CheckPoint;
                    _tourAttendanceService.Update(SelectedTourAttendance);
                }
                else
                {
                    MessageBox.Show("Guest is already present!", "Attendance", MessageBoxButton.OK);
                }
            }
        }

        private void MarkCurrentCheckPoint(object obj)
        {
            if (obj is not RadioButton { DataContext: CheckPointViewModel selectedCheckPoint } selectedRadioButton) return;

            selectedRadioButton.IsEnabled = false;

            _checkPointService.UpdateByTourId(selectedCheckPoint.Id, SelectedTour.Id);

            if (selectedCheckPoint.SerialNumber != 2) return;

            MessageBox.Show("Tour has successfully ended!", "TourAttendance", MessageBoxButton.OK,
                MessageBoxImage.Information);

            foreach (var tourDate in SelectedTour.TourDatesViewModel.TourDates.Where(t => t.TourId == SelectedTour.Id))
            {
                var dateOnly = GetSystemDateOnly();
                DateTime? tourDateOnly = ExtractTourDate(tourDate);

                if (!tourDateOnly.Equals(dateOnly)) continue;
                tourDate.HasEnded = true;
                _tourDateService.UpdateById(tourDate.Id);
            }

            _checkPointService.SetDefault();
            OnActionCompleted();
        }


        private static MessageBoxResult ConfirmEndTour()
        {
            const string messageBoxText = "Are you sure you want to end the tour prematurely?";
            const string messageBoxCaption = "Warning";
            const MessageBoxButton messageBoxButton = MessageBoxButton.YesNo;
            const MessageBoxImage messageBoxImage = MessageBoxImage.Warning;

            var result = MessageBox.Show(messageBoxText, messageBoxCaption, messageBoxButton, messageBoxImage);

            return result;
        }

        public static DateTime GetSystemDateOnly()
        {
            var dateTime = DateTime.Now;
            return dateTime.Date;
        }

        public static DateTime ExtractTourDate(TourDateViewModel tourDate)
        {
            DateTime? date = tourDate.Date;
            return date.Value.Date;
        }

        private void OpenGuideOverview(object obj)
        {
            MainWindow mainWindow = new(LoggedInUser);
            mainWindow.Show();
            OnActionCompleted();
        }

        public static event EventHandler<ActionCompletedEventArgs> ActionCompleted;

        private void OnActionCompleted()
        {
            var args = new ActionCompletedEventArgs();
            ActionCompleted?.Invoke(this, args);
        }
    }
}
