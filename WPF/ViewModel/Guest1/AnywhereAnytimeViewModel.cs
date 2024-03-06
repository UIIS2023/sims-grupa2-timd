using SimsProject.Application.Interface;
using SimsProject.Application.Localization;
using SimsProject.Domain.Model;
using SimsProject.WPF.View.Guest1;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace SimsProject.WPF.ViewModel.Guest1
{
    public class AnywhereAnytimeViewModel : ViewModelBase, IDataErrorInfo
    {
        public User CurrentUser { get; set; }
        public NavigationService NavigationService { get; set; }
        public AccommodationReservation SelectedReservation { get; set; }
        public ObservableCollection<AccommodationReservation> Reservations { get; set; }
        private readonly IAccommodationReservationService _accommodationReservationService;
        private readonly IAccommodationService _accommodationService;
        private readonly IGuest1Service _guest1Service;
        private readonly IAccommodationImageService _imageService;
        public AnywhereAnytimeViewModel(User currentUser, NavigationService navigationService)
        {
            CurrentUser = currentUser;
            NavigationService = navigationService;
            SelectedReservation = null;
            Reservations = new ObservableCollection<AccommodationReservation>();
            _accommodationReservationService = Injector.CreateInstance<IAccommodationReservationService>();
            _accommodationService = Injector.CreateInstance<IAccommodationService>();
            _guest1Service = Injector.CreateInstance<IGuest1Service>();
            _imageService = Injector.CreateInstance<IAccommodationImageService>();
            ReserveCommand = new RelayCommand(ReserveClick, CanReserveClick);
            SearchCommand = new RelayCommand(SearchClick, CanSearchClick);
            ImageCommand = new RelayCommand(ImageClick, CanImageClick);
            DGCommand = new RelayCommand(DGClick);
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
            return SelectedReservation != null;
        }
        public void ImageClick(object obj)
        {
            ObservableCollection<Image> images = new ObservableCollection<Image>();
            foreach (var im in SelectedReservation.Accommodation.Images)
            {
                images.Add(_imageService.GetById(im.Id));
            }
            ImageViewer form = new(images, false);
            form.ShowDialog();
        }
        private bool IsInputValid()
        {
            return int.TryParse(GuestNumber, out _) && int.TryParse(StayLength, out _) && (Date1 is null || Date1 >= DateTime.Today) && (Date2 is null || Date2 >= DateTime.Today) && (Date1 is null || Date2 is null || Date2 > Date1);
        }
        public ICommand SearchCommand { get; set; }
        public bool CanSearchClick(object obj)
        {
            return IsInputValid();
        }
        public void SearchClick(object obj)
        {
            var d1 = DateTime.MinValue;
            if (Date1 is not null) d1 = (DateTime)Date1;
            var d2 = DateTime.MinValue;
            if (Date2 is not null) d2 = (DateTime)Date2;
            ObservableCollection<AccommodationReservation> newReservations = new(_accommodationReservationService.SearchAnywhereAnytime(Convert.ToInt32(GuestNumber), Convert.ToInt32(StayLength), DateOnly.FromDateTime(d1), DateOnly.FromDateTime(d2)));
            Reservations.Clear();
            if (newReservations.Count == 0)
            {
                MessageBox.Show(TranslationSource.Instance["AAE"]);
                return;
            }
            foreach (var reservation in newReservations)
            {
                // hci
                Reservations.Add(reservation);
            }
        }
        public ICommand ReserveCommand { get; set; }
        public bool CanReserveClick(object obj)
        {
            return SelectedReservation != null;
        }
        public void ReserveClick(object obj)
        {
            if (ConfirmReservation() == MessageBoxResult.Yes)
            {
                _guest1Service.DeductPoint(CurrentUser);
                _accommodationReservationService.CreateReservation(_accommodationService.GetById(SelectedReservation.Accommodation.Id), CurrentUser, SelectedReservation.ArrivalDate, Convert.ToInt32(StayLength), Convert.ToInt32(GuestNumber));
                Reservations.Clear();
                SelectedReservation = null;
            }
        }
        private MessageBoxResult ConfirmReservation()
        {
            var sMessageBoxText = TranslationSource.Instance["AYS"] + " " + SelectedReservation.Accommodation.Name + " " + TranslationSource.Instance["From"] + " " + SelectedReservation.ArrivalDate.ToString("dd.MM.yyyy.") + " " + TranslationSource.Instance["To"] + " " + SelectedReservation.CheckoutDate.ToString("dd.MM.yyyy.") + " ?";
            var sCaption = TranslationSource.Instance["Confirmation"];
            var btnMessageBox = MessageBoxButton.YesNo;
            return MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox);
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
                    case nameof(StayLength):
                        if (!int.TryParse(StayLength, out _))
                        {
                            error = TranslationSource.Instance["ERR1"];
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
                        if (string.IsNullOrEmpty(GuestNumber))
                        {
                            error = TranslationSource.Instance["ERR4"];
                        }
                        break;
                    case nameof(Date1):
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
