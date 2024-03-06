using SimsProject.Application.UseCase;
using SimsProject.Domain.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimsProject.Repository;
using SimsProject.WPF.View;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows;
using System.Xml.Linq;
using System.Windows.Input;
using SimsProject.WPF.View.Guest2;
using System.Windows.Navigation;

namespace SimsProject.WPF.ViewModel.Guest2
{
    public class ExploreViewModel : INotifyPropertyChanged
    {

        public static ObservableCollection<Tour> Tours { get; set; }
        public List<Tour> AllTours { get; set; }
        public NavigationService service;
        public List<TourReservation> TourReservations { get; set; }
        public List<TourAttendance> TourAttendances { get; set; }
        public static Tour SelectedTour { get; set; }
        public User LoggedInUser { get; set; }


        private readonly TourReservationService _tourReservationService;
        private readonly LocationService _locationService;
        private readonly CheckPointService _checkPointService;
        private readonly TourDateService _tourDateService;
        private readonly TourImageService _imageService;
        private readonly UserService _userService;
        private readonly TourService _tourService;
       

        private List<String> _cityList;
        public List<String> CityList
        {
            get { return _cityList; }
            set
            {
                _cityList = value;
                OnPropertyChanged(nameof(CityList));
            }
        }
        private List<String> _countryList;

        public event PropertyChangedEventHandler PropertyChanged;

        public List<String> CountryList
        {
            get { return _countryList; }
            set
            {
                _countryList = value;
                OnPropertyChanged(nameof(CountryList));
            }
        }

        private string _TBLanguage;
        public string TBLanguage
        {
            get { return _TBLanguage; }
            set
            {
                _TBLanguage = value;
                OnPropertyChanged(nameof(TBLanguage));
            }
        }

       

        private int _selectedCountryIndex;
        public int SelectedCountryIndex
        {
            get => _selectedCountryIndex;
            set
            {
                _selectedCountryIndex = value;
                OnPropertyChanged(nameof(SelectedCountryIndex));
                OnCountrySelectionChanged();
            }
        }
        private int _selectedCityIndex;
        public int SelectedCityIndex
        {
            get => _selectedCityIndex;
            set
            {
                _selectedCityIndex = value;
                OnPropertyChanged(nameof(SelectedCityIndex));
            }
        }

        private string _TBDuration;
        public string TBDuration
        {
            get { return _TBDuration; }
            set
            {
                _TBDuration = value;
                OnPropertyChanged(nameof(TBDuration));
            }
        }

        private string _TBNumGuest;
        public string TBNumGuest
        {
            get { return _TBNumGuest; }
            set
            {
                _TBNumGuest = value;
                OnPropertyChanged(nameof(TBNumGuest));
            }
        }

        private string _selectedCity;
        public string SelectedCity
        {
            get { return _selectedCity; }
            set
            {
                _selectedCity = value;
                OnPropertyChanged(nameof(SelectedCity));
            }
        }

        private string _selectedCountry;
        public string SelectedCountry
        {
            get { return _selectedCountry; }
            set
            {
                _selectedCountry = value;
                OnPropertyChanged(nameof(SelectedCountry));
            }
        }

        private ICommand _searchCommand;
        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new RelayCommand(
                        param => this.SearchClick(),
                        param => this.CanSearchClick()
                    );
                }
                return _searchCommand;
            }
        }
        public bool CanSearchClick()
        {
            return true;
        }

        private ICommand _reserveCommand;
        public ICommand ReserveCommand
        {
            get
            {
                if (_reserveCommand == null)
                {
                    _reserveCommand = new RelayCommand(
                        param => this.ReserveClick(),
                        param => this.CanReserveClick()
                    );
                }
                return _reserveCommand;
            }
        }
        public bool CanReserveClick()
        {
            return SelectedTour!=null;
        }


        public ExploreViewModel(User user,NavigationService _service) {

            LoggedInUser = user;
            service=_service;
            _tourService = new TourService();
            _locationService = new LocationService();
            _checkPointService = new CheckPointService();
            _tourDateService = new TourDateService();
            _userService = new UserService();
            _tourReservationService = new TourReservationService();
            _imageService = new TourImageService();
            Tours = new ObservableCollection<Tour>(_tourService.GetAll());
            TourReservations = new(_tourReservationService.GetAll());

            Tours = new ObservableCollection<Tour>(_tourService.GetAll());
            ObservableCollection<string> Countries = new(_locationService.GetDistinctCountries());
            ObservableCollection<string> Cities = new(_locationService.GetDistinctCities());

     

            PopulateTours();
            GetSortedCheckPoints();
            CountryList = _locationService.GetDistinctCountries();
            CountryList.Insert(0,"Any");
            CityList = _locationService.GetDistinctCities();
            CityList.Insert(0, "Any");

        }

        

        private void PopulateTours()
        {
            foreach (var tour in Tours)
            {
                tour.TourLocation = _locationService.GetById(tour.TourLocation.Id);
                tour.CheckPoints = _checkPointService.GetByParentId(tour.Id);
                tour.Guide = _userService.GetById(tour.Guide.Id);
                tour.TourDates = _tourDateService.GetByParentId(tour.Id);
                tour.Images = _imageService.GetByParentId(tour.Id);
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
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private void UpdateTable()
        {
            Tours.Clear();
            foreach (var tour in _tourService.GetAll())
            {
                if (IsEnteredDataValid(tour) )
                {
                    Tours.Add(tour);
                    PopulateTours();
                    GetSortedCheckPoints();
                }

            }
        }
        public bool IsEnteredDataValid(Tour tour)
        {
            var isLanguageValid = string.IsNullOrEmpty(TBLanguage) || tour.Language.ToLower().Contains(TBLanguage.ToLower());
            var isGuestNumberValid = string.IsNullOrEmpty(TBNumGuest) || tour.MaxGuestNumber >= Convert.ToInt32(TBNumGuest);
            var isDurationValid = string.IsNullOrEmpty(TBDuration) || tour.Duration == Convert.ToInt32(TBDuration);
            var isCountryValid = string.IsNullOrEmpty(SelectedCountry) || SelectedCountry == "Any" || _locationService.GetById(tour.TourLocation.Id).Country == SelectedCountry;
            var isCityValid = string.IsNullOrEmpty(SelectedCity) || SelectedCity == "Any" || _locationService.GetById(tour.TourLocation.Id).City == SelectedCity;

            return isLanguageValid && isGuestNumberValid && isDurationValid  && isCountryValid && isCityValid;
        }
        private void SearchClick()
        {
               
                UpdateTable();
        }
        private void ReserveClick()
        {
                ReserveTourForm form = new(SelectedTour, LoggedInUser,service);
                form.ShowDialog();
        }
        private void OnCountrySelectionChanged()
        {
            List<string> cities;
            if (SelectedCountryIndex > 0)
            {
                
                var countries = _locationService.GetDistinctCountries();
                var c = countries[SelectedCountryIndex - 1];
                cities = _locationService.GetAllCitiesByCountry(c);
            }
            else
            {
                cities = new();
               
            }
            cities.Insert(0, "Any");
            CityList = cities;
            SelectedCityIndex = 0;
        }

    }
}
