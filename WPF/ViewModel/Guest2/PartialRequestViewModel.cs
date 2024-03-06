using SimsProject.Domain.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using SimsProject.WPF.View.Guest2;
using SimsProject.Application.UseCase;

namespace SimsProject.WPF.ViewModel.Guest2
{
    public class PartialRequestViewModel : ViewModelBase
    {
        public User LoggedInUser;
        public Page page;
        public List<User> Users;
        public List<User> Guides;
        public User Guide;
        public Location location;
        public LocationService _locationService;
        public TourRequestService _tourRequestService;
        public UserService _userService;
        public ObservableCollection<TourRequest> Requests { get; set; }

        private DateTime _startDate;
        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                if (value != _startDate)
                {
                    _startDate = value;
                    OnPropertyChanged(nameof(StartDate));
                }
            }
        }

        private DateTime _date1;
        public DateTime Date1
        {
            get => _date1;
            set
            {
                if (value != _date1)
                {
                    _date1 = value;
                    OnPropertyChanged(nameof(Date1));
                }
            }
        }
        private DateTime _date2;
        public DateTime Date2
        {
            get => _date2;
            set
            {
                _date2 = value;
                OnPropertyChanged(nameof(Date2));
            }
        }


        private List<String> _guideList;
        public List<String> GuideList
        {
            get { return _guideList; }
            set
            {
                _guideList = value;
                OnPropertyChanged(nameof(GuideList));
            }
        }

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

        private string _TBDescription;
        public string TBDescription
        {
            get { return _TBDescription; }
            set
            {
                _TBDescription = value;
                OnPropertyChanged(nameof(TBDescription));
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

        private string _selectedGuide;
        public string SelectedGuide
        {
            get { return _selectedGuide; }
            set
            {
                _selectedGuide = value;
                OnPropertyChanged(nameof(SelectedGuide));
            }
        }
        public PartialRequestViewModel(User CurrentUser,ObservableCollection<TourRequest> requests,Page _page) {

            LoggedInUser = CurrentUser;
            Requests =new( requests);
            page = _page;

            _locationService = new();
            _tourRequestService = new();
            _userService = new();
            Users = _userService.GetAll();
            Guides = new();
            Guide = new();
            location = new();
            FindGuides();

            CountryList = _locationService.GetDistinctCountries();
            CityList = _locationService.GetDistinctCities();
            SelectedCountryIndex = -1;
            GuideList = new();
            PopulateGuideList();
            StartDate = DateTime.Today.AddDays(2);

        }

        private ICommand _cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                {
                    _cancelCommand = new RelayCommand(
                        param => this.CancelClick(),
                        param => this.CanCancelClick()
                    );
                }
                return _cancelCommand;
            }
        }
        public bool CanCancelClick()
        {

            return true;
        }

        public void CancelClick()
        {
            var navigationService = page.NavigationService;
            var form = new CreateComplexRequest(LoggedInUser,Requests);
            navigationService.Navigate(form);

        }
        private ICommand _addCommand;
        public ICommand AddCommand
        {
            get
            {
                if (_addCommand == null)
                {
                    _addCommand = new RelayCommand(
                        param => this.AddClick(),
                        param => this.CanAddClick()
                    );
                }
                return _addCommand;
            }
        }
        public bool CanAddClick()
        {

            return true;
        }

        public void AddClick() {
            //dodaj request i prebaci na prethodnu stranicu
            location = _locationService.GetByCityAndCountry(SelectedCity, SelectedCountry);
            Guide = _userService.GetByUsername(SelectedGuide);
            Guide.Username = SelectedGuide;
            var Request = new TourRequest(TBDescription, Guide, TBLanguage, location, Convert.ToInt32(TBNumGuest), Date1, Date2, 0,new ComplexTourRequest());
            Requests.Add(Request);
           // CreateComplexRequestViewModel.Requests.Add(Request);
            var navigationService = page.NavigationService;
            var form = new CreateComplexRequest(LoggedInUser,Requests);
            navigationService.Navigate(form);

        }

        private void OnCountrySelectionChanged()
        {
            List<string> cities;
            if (SelectedCountryIndex > -1)
            {

                var countries = _locationService.GetDistinctCountries();
                var c = countries[SelectedCountryIndex];
                cities = _locationService.GetAllCitiesByCountry(c);
            }
            else
            {
                cities = new();

            }
            CityList = cities;
            SelectedCityIndex = -1;
        }

        public void FindGuides()
        {
            foreach (User user in Users)
            {
                if ((int)(user.Type) == 1)
                {
                    Guides.Add(user);
                }
            }
        }
        public void PopulateGuideList()
        {

            foreach (User guide in Guides)
            {
                GuideList.Add(guide.Username);
            }
        }
    }
}
