using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimsProject.Domain.Model;
using SimsProject.Application.UseCase;
using System.Globalization;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using SimsProject.WPF.View.Guest2;
using System.Windows.Input;
using System.Windows.Navigation;

namespace SimsProject.WPF.ViewModel.Guest2
{
    public  class ReserveTourFormViewModel : INotifyPropertyChanged
    {
        public Tour SelectedTour;
        public TourVoucher UsedVoucher;
        public int Occupacy;
        public User LoggedInUser;
        public TourReservation NewReservation;

        public TourReservationService tourReservationService;
        public ObservableCollection<TourReservation> tourReservations;
        private readonly TourDateService _tourDateService;
        private readonly TourVoucherService tourVoucherService;
        public List<TourDate> TourDates { get; set; }
        public List<TourVoucher> TourVouchers { get; set; }

        public Window form;
        public NavigationService Service;
     

        public event PropertyChangedEventHandler PropertyChanged;


        private List<String> _dateList;
        public List<String> DateList
        {
            get { return _dateList; }
            set
            {
                _dateList = value;
                OnPropertyChanged(nameof(DateList));
            }
        }

        private string _TBAge;
        public string TBAge
        {
            get { return _TBAge; }
            set
            {
                _TBAge = value;
                OnPropertyChanged(nameof(TBAge));
            }
        }

        private int _selectedVoucher;
        public int SelectedVoucher
        {
            get { return _selectedVoucher; }
            set
            {
                _selectedVoucher = value;
                OnPropertyChanged(nameof(SelectedVoucher));
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

        private string _selectedDate;
        public string SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                _selectedDate = value;
                OnPropertyChanged(nameof(SelectedDate));
            }
        }

       

        private List<String> _voucherList;
        public List<String> VoucherList
        {
            get { return _voucherList; }
            set
            {
                _voucherList = value;
                OnPropertyChanged(nameof(VoucherList));
            }
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

       

        public bool IsGuestNumValid() {
         
            var guestNumberValid = int.TryParse(TBNumGuest, out _);
            if (!guestNumberValid || string.IsNullOrEmpty(TBNumGuest)) { 
                MessageBox.Show("GuestNumber must be a whole number!");
                return false;
            }
            return true;
        }

        public bool IsAgeValid()
        {

            var ageValid = int.TryParse(TBAge, out _);
            if (!ageValid || string.IsNullOrEmpty(TBAge)) { 
                MessageBox.Show("Age must be a whole number!");
                return false;
            }
            return true;
        }

        
        public bool CanReserveClick()
        {
            
            return true;
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

        private ICommand _pdfCommand;
        public ICommand PDFCommand
        {
            get
            {
                if (_pdfCommand == null)
                {
                    _pdfCommand = new RelayCommand(
                        param => this.PDFClick(),
                        param => this.CanPDFClick()
                    );
                }
                return _pdfCommand;
            }
        }
        public bool CanPDFClick()
        {
            return true;
        }

        public void PDFClick() {
            PDFGuest2ViewModel.GenerateTourReservationPDF(SelectedTour, NewReservation);
        }
        public ReserveTourFormViewModel(Tour SelectedTour, User LoggedInUser, Window ReserveTourForm,NavigationService service) {
            this.SelectedTour = SelectedTour;
            this.LoggedInUser = LoggedInUser;
            form = ReserveTourForm;
            NewReservation = new();
            Service = service;
            tourReservationService = new TourReservationService();
            _tourDateService = new TourDateService();
            tourVoucherService = new TourVoucherService();
            TourDates = _tourDateService.GetByParentId(SelectedTour.Id);
            tourReservations = new(tourReservationService.GetAll());
            TourVouchers = new(tourVoucherService.GetByGuest(LoggedInUser));
            DateList = new List<string>();
            VoucherList = new List<string>();
            UsedVoucher = tourVoucherService.GetById(SelectedVoucher);
            FillDateList();
            FillVoucherList();


        }

        private void FillDateList() {
            foreach (TourDate tourDate in TourDates)
            {
                if (tourDate.Date.CompareTo(DateTime.Today) >0)
                {
                    string dateString = (tourDate.Date).ToString("dd/MM/yyyy HH:mm:ss ", CultureInfo.CurrentCulture);
                    DateList.Add(dateString);
                    SelectedDate = dateString;
                }

            }
        }

        private void FillVoucherList() {

            foreach (TourVoucher voucher in TourVouchers) {
                if (voucher.Guest.Id==LoggedInUser.Id && voucher.ValidUntil.CompareTo(DateTime.Today)>0) {
                    VoucherList.Add(voucher.Id.ToString());
                }
              
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private MessageBoxResult ConfirmReservation()
        {
            string sMessageBoxText = $"Are you sure you want to make reservation for {SelectedDate} ?";
            string sCaption = "Confirmation";
            MessageBoxButton btnMessageBox = MessageBoxButton.YesNo;
            
            return MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox);
        }

        private int CurrentTourOccupacy(Tour SelectedTour)
        {
            return tourReservationService.CurrentTourOccupancy(SelectedTour,SelectedDate);
        }
        private int CountFreePlace(Tour tour)
        {
            return tourReservationService.CountFreePlace(tour,SelectedTour,SelectedDate);
        }

        
        private bool IsEnteredDataValid()
        {
            
                var ValidMaxGuestNum = SelectedTour.MaxGuestNumber >= Convert.ToInt32(TBNumGuest);
                var HasFreePlace = Convert.ToInt32(TBNumGuest) <= CountFreePlace(SelectedTour);
                return ValidMaxGuestNum && HasFreePlace;
                

        }


        private void ReserveClick()
        {

            if (IsGuestNumValid() && IsAgeValid())
            {
                if (IsEnteredDataValid())
                {

                  
                        MessageBoxResult result = ConfirmReservation();
                        if (result == MessageBoxResult.Yes)
                        {
                            NewReservation= tourReservationService.CreateReservation(SelectedTour, LoggedInUser, TBNumGuest, SelectedDate, TBAge, tourVoucherService.GetById(SelectedVoucher));
                            MessageBoxResult pdf = ConfirmPDF();
                             if (pdf == MessageBoxResult.Yes)
                             {
                                 PDFGuest2ViewModel.GenerateTourReservationPDF(SelectedTour, NewReservation);
                             }
                            form.Close();
                            
                        }

                   



                 
                    
                }

                else
                {

                    ShowOtherOptions();
                }
            }
        }

        public void ShowOtherOptions() {
            if (CountFreePlace(SelectedTour) == 0)
            {
                MessageBoxResult result = TourIsFull();
                if (result == MessageBoxResult.Yes)
                {
                    form.Close();
                    RecommendedTour recommended = new RecommendedTour(LoggedInUser, SelectedTour);
                    Service.Navigate(recommended);
                }
            }
            else
            {
                int Occupacy = CountFreePlace(SelectedTour);
                MessageBox.Show(" There is no place for entered Guest Number. Current free places: " + Occupacy);
            }

        }

        private void CancelClick()
        {
            form.Close();
        }

        private MessageBoxResult TourIsFull()
        {
            string sMessageBoxText = $"Do you want to see other tours on same location ?";
            string sCaption = "NO FREE PLACE ON SELECTED TOUR FOR SELECTED DATE!";
            MessageBoxButton btnMessageBox = MessageBoxButton.YesNo;
            return MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox);
        }
        private MessageBoxResult ConfirmPDF()
        {
            string sMessageBoxText = $"Do You want to see PDF report ?";
            string sCaption = "See report";
            MessageBoxButton btnMessageBox = MessageBoxButton.YesNo;

            return MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox);
        }
    }
}
