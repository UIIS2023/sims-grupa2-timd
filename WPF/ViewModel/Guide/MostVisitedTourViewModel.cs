using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.WPF.ViewModel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace SimsProject.WPF.ViewModel.Guide
{
    public class MostVisitedTourViewModel : ViewModelBase
    {
        #region PROPERTIES

        public List<TourReservationViewModel> TourReservations { get; set; }
        public List<TourDateViewModel> TourDates { get; set; }
        public List<TourViewModel> Tours { get; set; }
        public List<string> Years { get; set; }

        private string _selectedYear;
        public string SelectedYear
        {
            get => _selectedYear;
            set
            {
                if (value != _selectedYear)
                {
                    _selectedYear = value;
                    OnPropertyChanged(nameof(SelectedYear));
                }
            }
        }

        private string _tbName;
        public string TbName
        {
            get => _tbName;
            set
            {
                if (value != _tbName)
                {
                    _tbName = value;
                    OnPropertyChanged(nameof(TbName));
                }
            }
        }

        private string _tbLocation;
        public string TbLocation
        {
            get => _tbLocation;
            set
            {
                if (value != _tbLocation)
                {
                    _tbLocation = value;
                    OnPropertyChanged(nameof(TbLocation));
                }
            }
        }

        private string _tbLanguage;
        public string TbLanguage
        {
            get => _tbLanguage;
            set
            {
                if (value != _tbLanguage)
                {
                    _tbLanguage = value;
                    OnPropertyChanged(nameof(TbLanguage));
                }
            }
        }

        private string _tbDescription;
        public string TbDescription
        {
            get => _tbDescription;
            set
            {
                if (value != _tbDescription)
                {
                    _tbDescription = value;
                    OnPropertyChanged(nameof(TbDescription));
                }
            }
        }


        private string _tbGuestNumber;
        public string TbGuestNumber
        {
            get => _tbGuestNumber;
            set
            {
                if (value != _tbGuestNumber)
                {
                    _tbGuestNumber = value;
                    OnPropertyChanged(nameof(TbGuestNumber));
                }
            }
        }

        private BitmapImage _tourImage;
        public BitmapImage TourImage
        {
            get => _tourImage;
            set
            {
                if (value != _tourImage)
                {
                    _tourImage = value;
                    OnPropertyChanged(nameof(TourImage));
                }
            }
        }

        private TourViewModel Tour { get; set; }
        private int GuestNumber { get; set; }
        private User LoggedInUser { get; }

        #endregion

        #region SERVICES

        private ITourService _tourService;
        private ITourReservationService _tourReservationService;
        private ITourDateService _tourDateService;

        #endregion

        #region COMMANDS

        public ICommand OpenFindTourCommand { get; set; }
        public ICommand OpenGuideOverviewCommand { get; set; }

        #endregion

        #region EVENTS

        public static event EventHandler<ActionCompletedEventArgs> ActionCompleted;
        public event EventHandler<string> ShowMessageBox;

        #endregion

        public MostVisitedTourViewModel(User user)
        {
            LoggedInUser = user;

            InitializeServices();
            InitializeCollections();
            InitializeCommands();
            FindDistinctYears();
        }

        private void InitializeCommands()
        {
            OpenGuideOverviewCommand = new RelayCommand(OpenGuideOverview);
            OpenFindTourCommand = new RelayCommand(OpenFindTour);
        }

        private void InitializeServices()
        {
            _tourReservationService = Injector.CreateInstance<ITourReservationService>();
            _tourDateService = Injector.CreateInstance<ITourDateService>();
            _tourService = Injector.CreateInstance<ITourService>();
        }

        private void InitializeCollections()
        {
            Tours = new List<TourViewModel>();
            TourDates = new List<TourDateViewModel>();
            TourReservations = new List<TourReservationViewModel>();
            Years = new List<string>();

            foreach (var tour in _tourService.GetByGuide(LoggedInUser))
            {
                Tours.Add(new TourViewModel(tour));
            }

            foreach (var tourReservation in _tourReservationService.GetByGuide(LoggedInUser))
            {
                TourReservations.Add(new TourReservationViewModel(tourReservation));
            }

            foreach (var tourDate in _tourDateService.GetAll())
            {
                TourDates.Add(new TourDateViewModel(tourDate));
            }
        }

        private void FindDistinctYears()
        {
            Years.Add("All years");

            foreach (var year in _tourDateService.FindDistinctYears())
            {
                Years.Add(year);
            }
        }

        private void OpenFindTour(object obj)
        {
            if (SelectedYear is null)
            {
                const string sMessageBoxText = "Please select the year for which you want to see the statistics for";
                OnShowMessageBox(sMessageBoxText);
            }
            else
            {
                var filteredReservations = TourReservations.Where(r => SelectedYear.Equals(Years[0]) || r.Date.Year == int.Parse(SelectedYear));
                var groupedReservations = filteredReservations.GroupBy(r => r.Tour.Id).Select(g => new
                {
                    TourId = g.Key,
                    TotalGuests = g.Sum(r => r.GuestNumber)
                });
                var tourWithMostGuests = groupedReservations.MaxBy(g => g.TotalGuests);
                Tour = Tours.Find(t => t.Id == tourWithMostGuests.TourId);
                GuestNumber = tourWithMostGuests.TotalGuests;
                PopulateTour();
            }
        }

        private void PopulateTour()
        {
            TbName = "Name: " + Tour.Name;
            TbLocation = "Location: " + Tour.TourLocation;
            TbDescription = "Description: " + Tour.Description;
            TbLanguage = "Language: " + Tour.Language;
            TourImage = new BitmapImage(new Uri(Tour.Cover.Url));
            TbGuestNumber = "Total number of guests: " + GuestNumber;
        }

        private void OpenGuideOverview(object obj)
        {
            OnActionCompleted();
        }

        private void OnActionCompleted()
        {
            var args = new ActionCompletedEventArgs();
            ActionCompleted?.Invoke(this, args);
        }

        private void OnShowMessageBox(string sMessageBoxText)
        {
            ShowMessageBox?.Invoke(this, sMessageBoxText);
        }
    }
}
