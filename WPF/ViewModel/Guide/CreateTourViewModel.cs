 using Microsoft.Win32;
using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.WPF.ViewModel.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace SimsProject.WPF.ViewModel.Guide
{
    public class CreateTourViewModel : ViewModelBase, IDataErrorInfo
    {
        #region PROPERTIES

        public ObservableCollection<CheckPoint> TemporaryCheckPoints { get; set; }
        public ObservableCollection<TourDate> TemporaryTourDates { get; set; }
        public ObservableCollection<Image> Images { get; set; }
        public ObservableCollection<string> Countries { get; set; }
        public ObservableCollection<string> Cities => new(_locationService.GetAllCitiesByCountry(Country));
        public string[] LanguageList { get; set; }
        public TourRequestViewModel TourRequest { get; set; }
        public User LoggedInUser { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsCityEnabled { get; set; }
        public bool IsCountryEnabled { get; set; }
        public bool IsLanguageEnabled { get; set; }
        public Location MostWantedLocation { get; set; }
        public string MostWantedLanguage { get; set; }


        private MessageBoxResult _result;
        private List<string> _messages;
        private string _checkPointLocation;
        private string _description;
        private string _tourName;
        private string _language;
        private string _country;
        private string _city;
        private DateTime? _tourDate;
        private int _currentCheckPoint;
        private int _maxGuestNumber;
        private int _duration;
        private Tour _newTour;
        private CheckPoint _endCheckPoint;

        public string TourName
        {
            get => _tourName;
            set
            {
                if (value != _tourName)
                {
                    _tourName = value;
                    OnPropertyChanged(nameof(TourName));
                }
            }
        }
        public string Description
        {
            get => _description;
            set
            {
                if (value != _description)
                {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }
        public string TourLanguage
        {
            get => _language;
            set
            {
                if (value != _language)
                {
                    _language = value;
                    OnPropertyChanged(nameof(TourLanguage));
                }
            }
        }
        public int MaxGuestNumber
        {
            get => _maxGuestNumber;
            set
            {
                if (value != _maxGuestNumber)
                {
                    _maxGuestNumber = value;
                    OnPropertyChanged(nameof(MaxGuestNumber));
                }
            }
        }
        public DateTime? TourDate
        {
            get => _tourDate;
            set
            {
                if (value != _tourDate)
                {
                    _tourDate = value;
                    OnPropertyChanged(nameof(TourDate));
                }
            }
        }
        public int Duration
        {
            get => _duration;
            set
            {
                if (value != _duration)
                {
                    _duration = value;
                    OnPropertyChanged(nameof(Duration));
                }
            }
        }
        public string CheckPointLocation
        {
            get => _checkPointLocation;
            set
            {
                if (value != _checkPointLocation)
                {
                    _checkPointLocation = value;
                    OnPropertyChanged(nameof(CheckPointLocation));
                }
            }
        }
        public string City
        {
            get => _city;
            set
            {
                if (value != _city)
                {
                    _city = value;
                    OnPropertyChanged(nameof(City));
                }
            }
        }
        public string Country
        {
            get => _country;
            set
            {
                if (value != _country)
                {
                    _country = value;
                    OnPropertyChanged(nameof(Country));
                    OnPropertyChanged(nameof(Cities));
                    City = Cities[0];
                }
            }
        }

        public string Language
        {
            get => _language;
            set
            {
                if (value != _language)
                {
                    _language = value;
                    OnPropertyChanged(nameof(Language));
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

        private string _checkPointLabel;
        public string CheckPointLabel
        {
            get => _checkPointLabel;
            set
            {
                if (value != _checkPointLabel)
                {
                    _checkPointLabel = value;
                    OnPropertyChanged(nameof(CheckPointLabel));
                }
            }
        }

        private string _tourDateInput;
        public string TourDateInput
        {
            get => _tourDateInput;
            set
            {
                if (value != _tourDateInput)
                {
                    _tourDateInput = value;
                    OnPropertyChanged(nameof(TourDateInput));
                }
            }
        }

        #endregion

        #region SERVICES

        private ITourService _tourService;
        private ILocationService _locationService;
        private ITourDateService _tourDateService;
        private ICheckPointService _checkPointService;
        private ITourRequestService _tourRequestService;
        private IGuest2NotificationService _guest2NotificationService;

        #endregion

        #region COMMANDS
        public ICommand RegisterCommand { get; set; }
        public ICommand GoBackCommand { get; set; }
        public ICommand UploadImagesFromComputerCommand { get; set; }
        public ICommand UploadImagesFromUrlCommand { get; set; }
        public ICommand AddCheckPointCommand { get; set; }
        public ICommand AddTourDateCommand { get; set; }

        #endregion

        #region VALIDATION

        public string this[string columnName]
        {
            get
            {
                string error = null;

                switch (columnName)
                {
                    case nameof(Description):
                        if (string.IsNullOrEmpty(Description))
                        {
                            error = "Description is required";
                        }
                        break;

                    case nameof(TourName):
                        if (string.IsNullOrEmpty(TourName))
                        {
                            error = "Tour name is required";
                        }
                        break;

                    case nameof(TourDateInput):
                        if (!DateTime.TryParseExact(TourDateInput, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate) && TemporaryTourDates.Count == 0)
                        {
                            error = "Date is not in correct format";
                        }

                        if (string.IsNullOrEmpty(TourDateInput) && TemporaryTourDates.Count == 0)
                        {
                            error = "Date is required";
                        }

                        if (parsedDate < DateTime.Today && parsedDate != DateTime.MinValue)
                        {
                            error = "Date cannot be before today's date";
                        }

                        if (TourRequest != null && (parsedDate <= TourRequest.FromDate || parsedDate >= TourRequest.ToDate))
                        {
                            error = $"Date needs to be in this range: \n{TourRequest.FromDate} - {TourRequest.ToDate}";
                        }

                        if (!IsDateValid(parsedDate))
                        {
                            error = "You cannot schedule tour in this period, since there is already one registered";
                        }
                        break;

                    case nameof(CheckPointLocation):
                        if (TemporaryCheckPoints.Count < 2)
                        {
                            error = "You have to enter at least two check\n point locations";
                        }
                        break;
                }
                return error;
            }
        }

        public string Error => null;

        #endregion

        #region EVENTS

        public event EventHandler<string> ShowMessageBox;
        public event EventHandler<string> ShowMessageBoxResult;
        public event EventHandler<MessageBoxResult> MessageBoxClosed;
        public static event EventHandler<ActionCompletedEventArgs> ActionCompleted;

        #endregion

        public CreateTourViewModel(User user, TourRequestViewModel tourRequest, Location location, string language)
        {
            LoggedInUser = user;
            TourRequest = tourRequest;
            MostWantedLocation = location;
            MostWantedLanguage = language;

            InitializeServices();
            InitializeCollections();
            InitializeCommands();
            SetDefaultValues();
            CheckForRequests();
            CheckForLocationOrLanguage();
        }

        private void InitializeServices()
        {
            _tourRequestService = Injector.CreateInstance<ITourRequestService>();
            _checkPointService = Injector.CreateInstance<ICheckPointService>();
            _locationService = Injector.CreateInstance<ILocationService>();
            _tourDateService = Injector.CreateInstance<ITourDateService>();
            _tourService = Injector.CreateInstance<ITourService>();
            _guest2NotificationService = Injector.CreateInstance<IGuest2NotificationService>();
        }

        private void InitializeCollections()
        {
            Images = new ObservableCollection<Image>();
            Countries = new ObservableCollection<string>(_locationService.GetDistinctCountries());

            TemporaryCheckPoints = new ObservableCollection<CheckPoint>();
            TemporaryTourDates = new ObservableCollection<TourDate>();
            _currentCheckPoint = 0;

            _messages = new List<string>
            {
                "*Start check point: ",
                "*End check point: ",
                "Additional check points: "
            };

            LanguageList = new[] { "English", "Serbian", "Spanish", "Hungarian", "Italian" };
        }

        private void InitializeCommands()
        {
            UploadImagesFromComputerCommand = new RelayCommand(UploadImagesFromComputer);
            AddCheckPointCommand = new RelayCommand(AddCheckPoint, CanAddCheckPoint);
            UploadImagesFromUrlCommand = new RelayCommand(UploadImagesFromUrl);
            AddTourDateCommand = new RelayCommand(AddTourDate, CanAddDate);
            RegisterCommand = new RelayCommand(Register, CanRegister);
            GoBackCommand = new RelayCommand(GoBack);
        }

        private void CheckForRequests()
        {
            if (TourRequest == null) return;
            TourLanguage = TourRequest.Language;
            MaxGuestNumber = TourRequest.GuestNumber;
            City = TourRequest.Location.City;
            Country = TourRequest.Location.Country;
            Description = TourRequest.Description;
            IsEnabled = false;
            IsCityEnabled = false;
            IsCountryEnabled = false;
            IsLanguageEnabled = false;
        }

        private void CheckForLocationOrLanguage()
        {
            if (TourRequest != null) return;
            if (MostWantedLanguage is not null && MostWantedLocation is null)
            {
                Language = MostWantedLanguage;
                IsLanguageEnabled = false;
            }
            else if (MostWantedLanguage is null && MostWantedLocation is not null)
            {
                Country = MostWantedLocation.Country;
                City = MostWantedLocation.City;

                IsCityEnabled = false;
                IsCountryEnabled = false;
            }
            else if (MostWantedLocation is not null && MostWantedLanguage is not null)
            {
                Language = MostWantedLanguage;
                Country = MostWantedLocation.Country;
                City = MostWantedLocation.City;

                IsCityEnabled = false;
                IsCountryEnabled = false;
                IsLanguageEnabled = false;
            }
            else
            {
                IsCityEnabled = true;
                IsCountryEnabled = true;
                IsLanguageEnabled = true;
            }
        }

        private void SetDefaultValues()
        {
            Country = Countries[0];
            Language = LanguageList[0];
            CheckPointLabel = _messages[0];
            IsEnabled = true;
        }

        private void Register(object obj)
        {
            var location = _locationService.GetByCityAndCountry(City, Country);
            _newTour = new Tour(TourName, Description, TourLanguage, MaxGuestNumber, location, Duration, Images.ToList(), LoggedInUser);
            var savedTour = _tourService.CreateTour(_newTour);
            SaveCheckPoints(savedTour);
            SaveTourDates(savedTour);

            if (TourRequest is not null)
            {
                if (TourRequest.RequestStatus == TourRequestStatus.Pending)
                {
                    TourRequest.RequestStatus = TourRequestStatus.Accepted; 
                    _tourRequestService.UpdateById(TourRequest.Id);
                    SendNotificationToGuest(savedTour, TourRequest.Id);
                }
            }
            MainPanelViewModel.Tours.Add(new TourViewModel(savedTour));

            const string sMessageBoxText = "Tour has successfully been registered";
            OnShowMessageBox(sMessageBoxText);

            OnActionCompleted();
        }

        private void SendNotificationToGuest(Tour savedTour, int tourRequestId)
        {
            _guest2NotificationService.NotifyGuest2(savedTour, tourRequestId);
        }

        private void SaveTourDates(Tour savedTour)
        {
            foreach (var tourDate in TemporaryTourDates)
            {
                tourDate.Tour.Id = savedTour.Id;
                _newTour.TourDates.Add(tourDate);
                _tourDateService.Save(tourDate);
            }
        }

        private void SaveCheckPoints(Tour savedTour)
        {
            foreach (var checkPoint in TemporaryCheckPoints)
            {
                checkPoint.Tour.Id = savedTour.Id;
                _newTour.CheckPoints.Add(checkPoint);
                _checkPointService.Save(checkPoint);
            }
        }

        private bool CanAddDate(object obj)
        {
            return !string.IsNullOrEmpty(TourDateInput);
        }

        private void AddTourDate(object obj)
        {
            DateTime.TryParseExact(TourDateInput, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var parsedDate);

            ScheduleTourDate(parsedDate);
        }

        private void ScheduleTourDate(DateTime date)
        {
            TourDate tourDate = new()
            {
                Id = -1,
                Date = date,
                Tour = new Tour()
            };

            TemporaryTourDates.Add(tourDate);
            TourDateInput = string.Empty;
        }

        private bool IsDateValid(DateTime parsedDate)
        {
            return !_tourService.GetByGuide(LoggedInUser).SelectMany(tour => _tourDateService.GetByParentId(tour.Id)).Any(date => parsedDate.Equals(date.Date) && !date.HasEnded);
        }

        private bool CanAddCheckPoint(object obj)
        {
            return !string.IsNullOrEmpty(CheckPointLocation);
        }

        private void AddCheckPoint(object obj)
        {
            var checkPoint = new CheckPoint
            {
                Id = -1,
                Name = CheckPointLocation,
                SerialNumber = TemporaryCheckPoints.Count + 1,
                Tour = new Tour(),
                IsActive = false
            };

            ++_currentCheckPoint;
            TemporaryCheckPoints.Add(checkPoint);

            switch (_currentCheckPoint)
            {
                case 2:
                    _endCheckPoint = TemporaryCheckPoints[1];
                    break;
                case > 2:
                    TemporaryCheckPoints.Remove(_endCheckPoint);
                    TemporaryCheckPoints.Add(_endCheckPoint);
                    break;
            }

            CheckPointLabel = _currentCheckPoint == 1 ? _messages[1] : _messages[2];

            CheckPointLocation = string.Empty;
        }

        private void UploadImagesFromComputer(object obj)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files|*.jpeg;*.jpg;*.png",
                FilterIndex = 1,
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == false) return;

            try
            {
                foreach (var filename in openFileDialog.FileNames)
                {
                    var image = new Image { Url = new Uri(filename).AbsoluteUri };
                    Images.Add(image);
                }
            }
            catch (Exception exception)
            {
                var sMessageBoxText = "Error adding image from the computer: " + exception.Message;
                OnShowMessageBox(sMessageBoxText);
            }
        }

        private void UploadImagesFromUrl(object obj)
        {
            if (!IsUrlValid(Url))
            {
                var sMessageBoxText = "Invalid URL '" + Url + "'";
                OnShowMessageBox(sMessageBoxText);
                return;
            }

            try
            {
                var image = new Image { Url = Url };
                Images.Add(image);
            }
            catch (Exception)
            {
                var sMessageBoxText = "Error adding image from URL  '" + Url + "'";
                OnShowMessageBox(sMessageBoxText);
            }

            Url = "";
        }

        private static bool IsUrlValid(string url)
        {
            const string pattern = @"^(http|https)://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?$";
            return Regex.IsMatch(url, pattern);
        }

        private bool CanRegister(object obj)
        {
            return TemporaryTourDates.Count > 0 && Images.Count >= 1 && string.IsNullOrEmpty(this["Description"]) && string.IsNullOrEmpty(this["TourName"]) && (TemporaryCheckPoints.Count + 1 > 2);
        }

        private void GoBack(object obj)
        {
            const string text = "If you proceed, any unsaved changes you've made to this tour registration will be lost.\n" +
                                "Otherwise, click 'Cancel' to return to the registration form and save your changes";

            OnShowMessageBoxResult(text);
            if (_result != MessageBoxResult.OK) return;

            OnActionCompleted();
        }

        private void OnShowMessageBox(string sMessageBoxText)
        {
            ShowMessageBox?.Invoke(this, sMessageBoxText);
        }

        private void OnShowMessageBoxResult(string sMessageBoxText)
        {
            ShowMessageBoxResult?.Invoke(this, sMessageBoxText);
        }

        public void ConfirmAction(MessageBoxResult result)
        {
            MessageBoxClosed?.Invoke(this, result);
            _result = result;
        }

        private void OnActionCompleted()
        {
            var args = new ActionCompletedEventArgs();
            ActionCompleted?.Invoke(this, args);
        }
    }
}
