using SimsProject.Domain.Model;
using SimsProject.WPF.ViewModel.Domain;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using SimsProject.Application.Interface;
using SimsProject.Application.Localization;
using System.Windows.Navigation;

namespace SimsProject.WPF.ViewModel.Guest1
{
    public class RequestReservationMoveViewModel : ViewModelBase, IDataErrorInfo
    {
        public User CurrentUser;
        private readonly Window _window;
        public string DateRange { get; set; }
        public AccommodationReservationViewModel SelectedReservation { get; set; }
        private readonly IAccommodationReservationRescheduleService _accommodationReservationRescheduleService;
        private readonly IAccommodationReservationService _accommodationReservationService;
        public RequestReservationMoveViewModel(Window window, AccommodationReservationViewModel selectedReservation, User currentUser)
        {
            CurrentUser = currentUser;
            SelectedReservation = selectedReservation;
            _window = window;
            _accommodationReservationRescheduleService = Injector.CreateInstance<IAccommodationReservationRescheduleService>();
            _accommodationReservationService = Injector.CreateInstance<IAccommodationReservationService>();
            DateRange = SelectedReservation.ArrivalDate.ToString("dd.MM.yyyy.") + " - " + SelectedReservation.CheckoutDate.ToString("dd.MM.yyyy.");
            RequestCommand = new RelayCommand(RequestClick, CanRequestClick);
            GoBackCommand = new RelayCommand(GoBackClick);
        }
        public ICommand GoBackCommand { get; set; }
        public void GoBackClick(object obj)
        {
            _window.Close();
        }

        public ICommand RequestCommand { get; set; }
        public bool CanRequestClick(object obj)
        {
            return Date1 is not null && Date2 is not null && Date1 <= Date2 && Date1 >= DateTime.Today;
        }
        public void RequestClick(object obj)
        {
            _accommodationReservationRescheduleService.CreateRequest(_accommodationReservationService.GetById(SelectedReservation.Id), CurrentUser, SelectedReservation.Accommodation.Owner, DateOnly.FromDateTime((DateTime)Date1), DateOnly.FromDateTime((DateTime)Date2));
            MessageBox.Show(TranslationSource.Instance["RequestSent"]);
            _window.Close();
        }
        private DateTime? _date1;
        public DateTime? Date1
        {
            get => _date1;
            set
            {
                if (value != _date1)
                {
                    _date1 = value;
                    OnPropertyChanged(nameof(Date1));
                    OnPropertyChanged(nameof(Date2));
                }
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

        public string Error => null;
        public string this[string columnName]
        {
            get
            {
                string error = null;

                switch (columnName)
                {
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
