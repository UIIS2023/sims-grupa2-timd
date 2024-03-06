using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.WPF.View.Guide;
using SimsProject.WPF.ViewModel.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;

namespace SimsProject.WPF.ViewModel.Guide
{
    public class TourRequestsViewModel : ViewModelBase, IDataErrorInfo
    {

        #region PROPERTIES

        private User LoggedInUser { get; }
        public List<TourViewModel> Tours { get; set; }
        public ObservableCollection<TourRequestViewModel> TourRequests { get; set; }
        public ObservableCollection<string> Countries { get; set; }
        public ObservableCollection<string> Cities => new(_locationService.GetAllCitiesByCountry(Country));
        public string[] LanguageList { get; set; }

        private string _language;
        private string _country;
        private string _city;
        private string _guestNumber;
        private DateTime? _fromDate;
        private DateTime? _toDate;
        private bool _isCityEnabled;
        private string _toDateInput;
        private string _fromDateInput;

        public string ToDateInput
        {
            get => _toDateInput;
            set
            {
                if (value != _toDateInput)
                {
                    _toDateInput = value;
                    OnPropertyChanged(nameof(ToDateInput));
                }
            }
        }

        public string FromDateInput
        {
            get => _fromDateInput;
            set
            {
                if (value != _fromDateInput)
                {
                    _fromDateInput = value;
                    OnPropertyChanged(nameof(FromDateInput));
                }
            }
        }

        public bool IsCityEnabled
        {
            get => _isCityEnabled;
            set
            {
                _isCityEnabled = value;
                OnPropertyChanged(nameof(IsCityEnabled));
            }
        }

        public DateTime? FromDate
        {
            get => _fromDate;
            set
            {
                if (value != _fromDate)
                {
                    _fromDate = value;
                    OnPropertyChanged(nameof(FromDate));
                }
            }
        }

        public DateTime? ToDate
        {
            get => _toDate;
            set
            {
                if (value != _toDate)
                {
                    _toDate = value;
                    OnPropertyChanged(nameof(ToDate));
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
                    if (_country is not null)
                    {
                        IsCityEnabled = true;
                    }
                }
            }
        }

        public string GuestNumber
        {
            get => _guestNumber;
            set
            {
                if (value != _guestNumber)
                {
                    _guestNumber = value;
                    OnPropertyChanged(nameof(GuestNumber));
                }
            }
        }

        #endregion

        #region COMMANDS

        public ICommand OpenFindTourRequestCommand { get; set; }
        public ICommand OpenGuideOverviewCommand { get; set; }
        public ICommand AcceptTourRequestCommand { get; set; }
        public ICommand ClearParametersCommand { get; set; }

        #endregion

        #region SERVICES

        private ITourRequestService _tourRequestService;
        private ILocationService _locationService;
        private ITourService _tourService;

        #endregion

        #region VALIDATION

        public string this[string columnName]
        {
            get
            {
                string error = null;

                switch (columnName)
                {
                    case nameof(FromDateInput):
                        if (!DateTime.TryParseExact(FromDateInput, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out _) && FromDateInput != null)
                        {
                            error = "From date is not\n in correct format";
                        }
                        break;

                    case nameof(ToDateInput):
                        if (!DateTime.TryParseExact(ToDateInput, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out _) && ToDateInput != null)
                        {
                            error = "From date is not\n in correct format";
                        }
                        break;
                }
                return error;
            }
        }

        public string Error => null;

        #endregion

        #region EVENTS

        public static event EventHandler<ActionCompletedEventArgs> ActionCompleted;
        public event EventHandler<OpenWindowEventArgs> OpenWindowRequested;

        #endregion

        public TourRequestsViewModel(User user)
        {
            LoggedInUser = user;

            InitializeServices();
            InitializeCollections();
            InitializeCommands();
        }

        private void InitializeServices()
        {
            _tourRequestService = Injector.CreateInstance<ITourRequestService>();
            _locationService = Injector.CreateInstance<ILocationService>();
            _tourService = Injector.CreateInstance<ITourService>();
        }

        private void InitializeCommands()
        {
            OpenFindTourRequestCommand = new RelayCommand(OpenFindTourRequest, CanSearchClick);
            OpenGuideOverviewCommand = new RelayCommand(OpenGuideOverview);
            AcceptTourRequestCommand = new RelayCommand(AcceptTourRequest);
            ClearParametersCommand = new RelayCommand(ClearParameters);
        }

        private void InitializeCollections()
        {
            TourRequests = new ObservableCollection<TourRequestViewModel>();
            Tours = new List<TourViewModel>();

            foreach (var tourRequest in _tourRequestService.GetAll().Where(tourRequest => tourRequest.RequestStatus == TourRequestStatus.Pending))
            {
                TourRequests.Add(new TourRequestViewModel(tourRequest));
            }

            foreach (var tour in _tourService.GetByGuide(LoggedInUser))
            {
                Tours.Add(new TourViewModel(tour));
            }

            GuestNumber = "1";
            LanguageList = new[] { "English", "Serbian", "Spanish", "Hungarian", "Italian" };
            FillCountryComboBox();
        }

        private void AcceptTourRequest(object obj)
        {
            if (obj is not TourRequestViewModel selectedRequest) return;
            var createTour = new CreateTour(LoggedInUser, selectedRequest, null, null);
            OpenWindowRequested?.Invoke(this, new OpenWindowEventArgs(createTour));
            OnActionCompleted();
        }

        private void FillCountryComboBox()
        {
            Countries = new ObservableCollection<string>(_locationService.GetDistinctCountries());
        }

        private void OpenFindTourRequest(object obj)
        {
            TourRequests.Clear();
            foreach (var tourRequest in _tourRequestService.Search(Language, FromDateInput, ToDateInput, City, Country, GuestNumber))
            { 
                TourRequests.Add(new TourRequestViewModel(tourRequest));
            }
        }

        private bool CanSearchClick(object obj)
        {
            var hasErrors = !string.IsNullOrEmpty(this[nameof(FromDateInput)]) ||
                             !string.IsNullOrEmpty(this[nameof(ToDateInput)]);

            var isDateInputEmpty = string.IsNullOrEmpty(FromDateInput) &&
                                    string.IsNullOrEmpty(ToDateInput);

            return !hasErrors || isDateInputEmpty;
        }

        private void ClearParameters(object obj)
        {
            Language = null;
            Country = null;
            City = null;
            FromDateInput = null;
            ToDateInput = null;
            GuestNumber = "1";
            IsCityEnabled = false;
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
    }
}
