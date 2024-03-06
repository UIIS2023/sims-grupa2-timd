using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Navigation;
using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.WPF.ViewModel.Domain;

namespace SimsProject.WPF.ViewModel.Owner
{
    public class RegistrationPanelViewModel : ViewModelBase, IDataErrorInfo
    {
        public User Owner { get; set; }
        public NavigationService NavigationService { get; set; }

        #region SERVICES

        private IAccommodationService _accommodationService;
        private ILocationService _locationService;
        private IAccommodationTypeService _accommodationTypeService;

        #endregion

        #region PROPERTIES

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private string _country;
        public string Country
        {
            get => _country;
            set
            {
                if (_country != value)
                {
                    _country = value;
                    City = Cities[0];
                    OnPropertyChanged(nameof(Country));
                    OnPropertyChanged(nameof(Cities));
                }
            }
        }

        private string _city;
        public string City
        {
            get => _city;
            set
            {
                if (_city != value)
                {
                    _city = value;
                    OnPropertyChanged(nameof(City));
                }
            }
        }

        private AccommodationTypeViewModel _type;
        public AccommodationTypeViewModel Type
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged(nameof(Type));
                }
            }
        }

        private int _maxGuestNumber;
        public int MaxGuestNumber
        {
            get => _maxGuestNumber;
            set
            {
                if (_maxGuestNumber != value)
                {
                    _maxGuestNumber = value;
                    OnPropertyChanged(nameof(MaxGuestNumber));
                }
            }
        }

        private int _minReservationDays;
        public int MinReservationDays
        {
            get => _minReservationDays;
            set
            {
                if (_minReservationDays != value)
                {
                    _minReservationDays = value;
                    OnPropertyChanged(nameof(MinReservationDays));
                }
            }
        }

        private int _minDaysBeforeCancellation;
        public int MinDaysBeforeCancellation
        {
            get => _minDaysBeforeCancellation;
            set
            {
                if (_minDaysBeforeCancellation != value)
                {
                    _minDaysBeforeCancellation = value;
                    OnPropertyChanged(nameof(MinDaysBeforeCancellation));
                }
            }
        }

        private string _url;
        public string Url
        {
            get => _url;
            set
            {
                if (_url != value)
                {
                    _url = value;
                    OnPropertyChanged(nameof(Url));
                }
            }
        }

        public ObservableCollection<string> Countries { get; set; }
        public ObservableCollection<string> Cities => new(_locationService.GetAllCitiesByCountry(Country));
        public ObservableCollection<Image> Images { get; set; }
        public ObservableCollection<AccommodationTypeViewModel> Types { get; set; }

        #endregion

        #region COMMANDS

        public ICommand RegisterCommand { get; set; }
        public ICommand DiscardCommand { get; set; }
        public ICommand UploadImagesFromComputerCommand { get; set; }

        #endregion
        
        #region VALIDATION

        public string this[string columnName]
        {
            get
            {
                string error = null;

                switch (columnName)
                {
                    case nameof(Name):
                        if (string.IsNullOrEmpty(Name))
                        {
                            error = "Name is required";
                        }
                        break;

                    case nameof(Images):
                        if (!Images.Any())
                        {
                            error = "At least one image is required";
                        }
                        break;
                }
                return error;
            }
        }

        public string Error => null;

        private readonly string[] _canRegisterValidatedProperties = { "Name", "Images" };

        #endregion

        #region EVENTS

        public bool ConfirmActionResult { get; set; }
        public event EventHandler ConfirmActionRequested;
        public event EventHandler UploadImagesFromComputerRequested;

        #endregion

        public RegistrationPanelViewModel(User owner, NavigationService navigationService)
        {
            Owner = owner;
            NavigationService = navigationService;

            InitializeServices();
            InitializeCollections();
            InitializeCommands();
            SetDefaultValues();
        }

        private void InitializeServices()
        {
            _accommodationService = Injector.CreateInstance<IAccommodationService>();
            _locationService = Injector.CreateInstance<ILocationService>();
            _accommodationTypeService = Injector.CreateInstance<IAccommodationTypeService>();
        }

        private void InitializeCollections()
        {
            Countries = new ObservableCollection<string>(_locationService.GetDistinctCountries());
            Types = new ObservableCollection<AccommodationTypeViewModel>();
            foreach (var type in _accommodationTypeService.GetAll())
            {
                Types.Add(new AccommodationTypeViewModel(type));
            }
            Images = new ObservableCollection<Image>();
        }

        private void InitializeCommands()
        {
            RegisterCommand = new RelayCommand(Register, CanRegister);
            DiscardCommand = new RelayCommand(Discard);
            UploadImagesFromComputerCommand = new RelayCommand(UploadImagesFromComputer);
        }

        private void SetDefaultValues()
        {
            Country = Countries[0];
            Types[0].IsSelected = true;
            Type = Types[0];
            MaxGuestNumber = 1;
            MinReservationDays = 1;
            MinDaysBeforeCancellation = 1;
        }

        private void Register(object obj)
        {
            var location = _locationService.GetByCityAndCountry(City, Country);
            var type = _accommodationTypeService.GetById(Type.Id);
            var newAccommodation = new Accommodation(Name, location, type, MaxGuestNumber, MinReservationDays, MinDaysBeforeCancellation, Owner, Images.ToList());
            _accommodationService.CreateAccommodation(newAccommodation);
            NavigationService.GoBack();
        }

        private void UploadImagesFromComputer(object obj)
        {
            UploadImagesFromComputerRequested?.Invoke(this, EventArgs.Empty);
        }

        public void AddImagesFromComputer(string[] fileNames)
        {
            foreach (var filename in fileNames)
            {
                var image = new Image { Url = new Uri(filename).AbsoluteUri };
                Images.Add(image);
            }
        }

        public void AddImageFromWeb(string url)
        {
            if (!IsUrlValid(url)) throw new Exception();

            var image = new Image { Url = new Uri(url).AbsoluteUri };
            Images.Add(image);
        }

        public static bool IsUrlValid(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;

            var pattern = @"^(http|https)://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?$";
            return Regex.IsMatch(url, pattern);
        }

        private bool CanRegister(object obj)
        {
            return _canRegisterValidatedProperties.All(property => string.IsNullOrEmpty(this[property]));
        }

        private void Discard(object obj)
        {
            ConfirmActionRequested?.Invoke(this, EventArgs.Empty);
            if (ConfirmActionResult)
            {
                NavigationService.GoBack();
            }
        }
    }
}