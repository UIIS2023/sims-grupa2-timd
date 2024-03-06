using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Navigation;
using SimsProject.Application.Interface;
using SimsProject.Application.Localization;
using SimsProject.Domain.Model;
using SimsProject.WPF.View.Guest1;
using SimsProject.WPF.ViewModel.Domain;

namespace SimsProject.WPF.ViewModel.Guest1
{
    public class SearchPageViewModel : ViewModelBase, IDataErrorInfo
    {
        public User CurrentUser { get; set; }
        public NavigationService NavigationService { get; set; }
        public static AccommodationViewModel SelectedAccommodation { get; set; }
        public ObservableCollection<AccommodationViewModel> Accommodations { get; set; }
        private readonly IAccommodationTypeService _accommodationTypeService;
        private readonly IAccommodationService _accommodationService;
        private readonly ILocationService _locationService;
        private readonly IUserService _userService;
        private readonly IAccommodationImageService _imageService;
        public SearchPageViewModel(User currentUser, NavigationService navigationService)
        {
            CurrentUser = currentUser;
            NavigationService = navigationService;
            SelectedAccommodation = null;
            _accommodationTypeService = Injector.CreateInstance<IAccommodationTypeService>();
            _accommodationService = Injector.CreateInstance<IAccommodationService>();
            _locationService = Injector.CreateInstance<ILocationService>();
            _userService = Injector.CreateInstance<IUserService>();
            _imageService = Injector.CreateInstance<IAccommodationImageService>();
            Accommodations = new ObservableCollection<AccommodationViewModel>();
            foreach (var accommodation in _accommodationService.GetAll())
            {
                Accommodations.Add(new AccommodationViewModel(accommodation));
            }
            FillTypeComboBox();
            FillCountryComboBox();
            IsCityEnabled = false;
            SearchCommand = new RelayCommand(SearchClick, CanSearchClick);
            ReserveCommand = new RelayCommand(ReserveClick, CanReserveClick);
            ImageCommand = new RelayCommand(ImageClick, CanImageClick);
            DGCommand = new RelayCommand(DGClick);
            SearchClick(null);
        }
        public ICommand DGCommand { get; set; }
        public void DGClick(object obj)
        {
            if (obj.ToString() == "Return")
            {
                ImageClick(obj);
            }
        }
        public ICommand ImageCommand { get; set; }
        public bool CanImageClick(object obj)
        {
            return SelectedAccommodation != null;
        }
        public void ImageClick(object obj)
        {
            ObservableCollection<Image> images = new ObservableCollection<Image>();
            foreach (var im in SelectedAccommodation.Images)
            {
                images.Add(_imageService.GetById(im.Id));
            }
            ImageViewer form = new(images, false);
            form.ShowDialog();
        }

        public ICommand SearchCommand { get; set; }

        public bool CanSearchClick(object obj)
        {
            return IsInputValid();
        }
        public void SearchClick(object obj)
        {
            ObservableCollection<Accommodation> newAccommodations = new(_accommodationService.Search(Name, GuestNumber, StayLength, SelectedCity, SelectedCountry, SelectedType));
            Accommodations.Clear();
            foreach (var accommodation in newAccommodations)
            {
                RankAccommodations(accommodation);
            }
        }

        private void RankAccommodations(Accommodation accommodation)
        {
            var owner = _userService.GetById(accommodation.Owner.Id);
            if (owner.IsSuperUser)
            {
                Accommodations.Insert(0, new AccommodationViewModel(accommodation));
            }
            else
            {
                Accommodations.Add(new AccommodationViewModel(accommodation));
            }
        }

        public ICommand ReserveCommand { get; set; }
        public bool CanReserveClick(object obj)
        {
            return SelectedAccommodation != null;
        }
        public void ReserveClick(object obj)
        {
            NavigationService.Navigate(
                new Uri("/SimsProject;component/WPF/View/Guest1/ReserveAccommodationForm.xaml", UriKind.Relative));
        }
        private bool IsInputValid()
        {
            return (string.IsNullOrEmpty(GuestNumber) || int.TryParse(GuestNumber, out _)) && (string.IsNullOrEmpty(StayLength) || int.TryParse(StayLength, out _));
        }
        private void FillTypeComboBox()
        {
            var types = _accommodationTypeService.GetAll();
            types.Insert(0, new AccommodationType(0, "Any"));
            var temp = new List<string>();
            foreach (var t in types)
            {
                temp.Add(t.Name);
            }
            Types = temp;
            SelectedTypeIndex = 0;
        }
        private void FillCountryComboBox()
        {
            var countries = _locationService.GetDistinctCountries();
            countries.Insert(0, "Any");
            Countries = countries;
            SelectedCountryIndex = 0;
        }
        private void OnCountrySelectionChanged()
        {
            List<string> cities;
            if (SelectedCountryIndex > 0)
            {
                IsCityEnabled = true;
                var countries = _locationService.GetDistinctCountries();
                var c = countries[SelectedCountryIndex - 1];
                cities = _locationService.GetAllCitiesByCountry(c);
            }
            else
            {
                cities = new List<string>();
                IsCityEnabled = false;
            }
            cities.Insert(0, "Any");
            Cities = cities;
            SelectedCityIndex = 0;
        }

        private List<string> _countries;
        public List<string> Countries
        {
            get => _countries;
            set
            {
                if (_countries != value)
                {
                    _countries = value;
                    OnPropertyChanged(nameof(Countries));
                }
            }
        }

        private List<string> _cities;
        public List<string> Cities
        {
            get => _cities;
            set
            {
                if (_cities != value)
                {
                    _cities = value;
                    OnPropertyChanged(nameof(Cities));
                }
            }
        }

        private List<string> _types;
        public List<string> Types
        {
            get => _types;
            set
            {
                if (_types != value)
                {
                    _types = value;
                    OnPropertyChanged(nameof(Types));
                }
            }
        }

        private bool _isCityEnabled = true;
        public bool IsCityEnabled
        {
            get => _isCityEnabled;
            set
            {
                _isCityEnabled = value;
                OnPropertyChanged(nameof(IsCityEnabled));
            }
        }

        private string _selectedCountry;
        public string SelectedCountry
        {
            get => _selectedCountry;
            set
            {
                _selectedCountry = value;
                OnPropertyChanged(nameof(SelectedCountry));
            }
        }

        private string _selectedCity;
        public string SelectedCity
        {
            get => _selectedCity;
            set
            {
                _selectedCity = value;
                OnPropertyChanged(nameof(SelectedCity));
            }
        }

        private string _selectedType;
        public string SelectedType
        {
            get => _selectedType;
            set
            {
                _selectedType = value;
                OnPropertyChanged(nameof(SelectedType));
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

        private int _selectedTypeIndex;
        public int SelectedTypeIndex
        {
            get => _selectedTypeIndex;
            set
            {
                _selectedTypeIndex = value;
                OnPropertyChanged(nameof(SelectedTypeIndex));
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private string _guestNumber;
        public string GuestNumber
        {
            get => _guestNumber;
            set
            {
                _guestNumber = value;
                OnPropertyChanged(nameof(GuestNumber));
            }
        }

        private string _stayLength;
        public string StayLength
        {
            get => _stayLength;
            set
            {
                _stayLength = value;
                OnPropertyChanged(nameof(StayLength));
            }
        }
        public string Error => null;
        public string this[string columnName]
        {
            get
            {
                string error = null;

                switch (columnName)
                {
                    case nameof(StayLength):
                        if (!int.TryParse(StayLength, out _))
                        {
                            error = TranslationSource.Instance["ERR1"];
                        }
                        if (string.IsNullOrEmpty(StayLength))
                        {
                            error = null;
                        }
                        break;

                    case nameof(GuestNumber):
                        if (!int.TryParse(GuestNumber, out _))
                        {
                            error = TranslationSource.Instance["ERR3"];
                        }
                        if (string.IsNullOrEmpty(GuestNumber))
                        {
                            error = null;
                        }
                        break;
                }
                return error;
            }
        }
    }
}
