using SimsProject.Domain.Model;
using SimsProject.WPF.ViewModel.Domain;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using SimsProject.Application.Interface;
using SimsProject.Application.Localization;
using System.Windows.Navigation;
using SimsProject.Application.Dto;

namespace SimsProject.WPF.ViewModel.Guest1
{
    public class ReserveAccommodationViewModel : ViewModelBase, IDataErrorInfo
    {
        public User CurrentUser { get; set; }
        public NavigationService NavigationService { get; set; }
        public DateRangeDto SelectedDateRange { get; set; }
        public AccommodationViewModel SelectedAccommodation { get; set; }
        public ObservableCollection<DateRangeDto> ValidDays { get; set; }
        public ObservableCollection<AccommodationReservationViewModel> AccommodationReservations;
        private readonly IAccommodationReservationService _accommodationReservationService;
        private readonly IAccommodationService _accommodationService;
        private readonly IGuest1Service _guest1Service;
        public ReserveAccommodationViewModel(User currentUser, NavigationService navigationService)
        {
            CurrentUser = currentUser; 
            NavigationService = navigationService;
            SelectedAccommodation = SearchPageViewModel.SelectedAccommodation;
            AccommodationReservations = new ObservableCollection<AccommodationReservationViewModel>();
            ValidDays = new ObservableCollection<DateRangeDto>();
            _accommodationReservationService = Injector.CreateInstance<IAccommodationReservationService>();
            _accommodationService = Injector.CreateInstance<IAccommodationService>();
            _guest1Service = Injector.CreateInstance<IGuest1Service>();
            foreach (var acr in _accommodationReservationService.GetByAccommodation(SelectedAccommodation.Id))
            {
                AccommodationReservations.Add(new AccommodationReservationViewModel(acr));
            }
            SearchDatesCommand = new RelayCommand(SearchDatesClick, CanSearchDatesClick);
            ReserveCommand = new RelayCommand(ReserveClick, CanReserveClick);
            GoBackCommand = new RelayCommand(GoBackClick);
        }
        public ICommand GoBackCommand { get; set; }
        public void GoBackClick(object obj)
        {
            NavigationService.GoBack();
        }

        private bool IsInputValid()
        {
            if (!int.TryParse(StayLength, out _) || !int.TryParse(GuestNumber, out _))
                return false;
            if (Convert.ToInt32(StayLength) < SelectedAccommodation.MinReservationDays || Convert.ToInt32(GuestNumber) > SelectedAccommodation.MaxGuestNumber)
                return false;
            return Date1 <= Date2 && Date1 > DateTime.Today;
        }
        public ICommand SearchDatesCommand { get; set; }
        public bool CanSearchDatesClick(object obj)
        {
            return IsInputValid();
        }
        public void SearchDatesClick(object obj)
        {
            var availableDates = _accommodationReservationService.FindAvailableDates(DateOnly.FromDateTime((DateTime)Date1), DateOnly.FromDateTime((DateTime)Date2), Convert.ToInt32(StayLength), _accommodationService.GetById(SelectedAccommodation.Id));
            if (availableDates.Count == 0)
            {
                MessageBox.Show(TranslationSource.Instance["NAS"]);
                var recommendedDays = _accommodationReservationService.SearchRecommendedDates(DateOnly.FromDateTime((DateTime)Date1), DateOnly.FromDateTime((DateTime)Date2), Convert.ToInt32(StayLength), _accommodationService.GetById(SelectedAccommodation.Id));
                availableDates.AddRange(recommendedDays);
            }
            ValidDays.Clear();
            foreach (var dr in availableDates) { ValidDays.Add(dr); }
        }
        public ICommand ReserveCommand { get; set; }
        public bool CanReserveClick(object obj)
        {
            return SelectedDateRange != null;
        }
        public void ReserveClick(object obj)
        {
            if (ConfirmReservation() == MessageBoxResult.Yes)
            {
                _guest1Service.DeductPoint(CurrentUser);
                _accommodationReservationService.CreateReservation(_accommodationService.GetById(SelectedAccommodation.Id), CurrentUser, SelectedDateRange.StartDate, Convert.ToInt32(StayLength), Convert.ToInt32(GuestNumber));
                NavigationService.GoBack();
            }
        }
        private MessageBoxResult ConfirmReservation()
        {
            var sMessageBoxText = TranslationSource.Instance["AYS"] + " " + SelectedAccommodation.Name + " " + TranslationSource.Instance["From"] + " " + SelectedDateRange.StartDate.ToString("dd.MM.yyyy.") + " " + TranslationSource.Instance["To"] + " " + SelectedDateRange.EndDate.ToString("dd.MM.yyyy.") + " ?";
            var sCaption = TranslationSource.Instance["Confirmation"];
            var btnMessageBox = MessageBoxButton.YesNo;
            return MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox);
        }
        private DateTime? _date1;
        public DateTime? Date1
        {
            get => _date1;
            set
            {
                _date1 = value;
                OnPropertyChanged(nameof(Date1));
                OnPropertyChanged(nameof(Date2));
            }
        }
        private DateTime? _date2;
        public DateTime? Date2
        {
            get => _date2;
            set
            {
                _date2 = value;
                OnPropertyChanged(nameof(Date1));
                OnPropertyChanged(nameof(Date2));
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
                        else if (Convert.ToInt32(StayLength) < SelectedAccommodation.MinReservationDays)
                        {
                            error = TranslationSource.Instance["ERR9"] + SelectedAccommodation.MinReservationDays;
                        }
                        if (string.IsNullOrEmpty(StayLength))
                        {
                            error = TranslationSource.Instance["ERR2"];
                        }
                        break;

                    case nameof(GuestNumber):
                        if (!int.TryParse(GuestNumber, out _))
                        {
                            error = TranslationSource.Instance["ERR3"];
                        }
                        else if (Convert.ToInt32(GuestNumber) > SelectedAccommodation.MaxGuestNumber)
                        {
                            error = TranslationSource.Instance["ERR10"] + SelectedAccommodation.MaxGuestNumber;
                        }
                        if (string.IsNullOrEmpty(GuestNumber))
                        {
                            error = TranslationSource.Instance["ERR4"];
                        }
                        break;

                    case nameof(Date1):
                        if (Date1 is null)
                        {
                            error = TranslationSource.Instance["ERR8"];
                        }
                        if (Date1 < DateTime.Today)
                        {
                            error = TranslationSource.Instance["ERR5"];
                        }
                        if (Date2 < Date1)
                        {
                            error = TranslationSource.Instance["ERR6"];
                        }
                        break;

                    case nameof(Date2):
                        if (Date2 is null)
                        {
                            error = TranslationSource.Instance["ERR8"];
                        }
                        if (Date2 < DateTime.Today)
                        {
                            error = TranslationSource.Instance["ERR5"];
                        }
                        if (Date2 < Date1)
                        {
                            error = TranslationSource.Instance["ERR6"];
                        }
                        break;
                }
                return error;
            }
        }
    }
}
