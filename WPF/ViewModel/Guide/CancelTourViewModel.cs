using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.WPF.ViewModel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SimsProject.WPF.ViewModel.Guide
{
    public class CancelTourViewModel : ViewModelBase
    {
        #region PROPERTIES

        public List<TourReservation> TourReservations { get; set; }
        public List<User> Guests { get; set; }
        public TourDateViewModel SelectedDate { get; set; }
        public User LoggedInUser { get; set; }
        public TourViewModel Tour { get; set; }

        private MessageBoxResult _result;

        #endregion

        #region SERVICES

        private ITourReservationService _tourReservationService;
        private ICheckPointService _checkPointService;
        private ITourVoucherService _voucherService;
        private ITourDateService _tourDateService;
        private ITourImageService _imageService;

        #endregion

        #region COMMANDS

        public ICommand OpenGuideOverviewCommand { get; set; }
        public ICommand CancelTourCommand { get; set; }

        #endregion

        #region EVENTS

        public event EventHandler<string> ShowMessageBox;
        public event EventHandler<string> ShowMessageBoxResult;
        public event EventHandler<MessageBoxResult> MessageBoxClosed;
        public static event EventHandler<ActionCompletedEventArgs> ActionCompleted;

        #endregion

        public CancelTourViewModel(TourViewModel tour, User user)
        {
            LoggedInUser = user;
            Tour = tour;

            InitializeServices();
            InitializeCollections();
            InitializeCommands();
        }

        private void InitializeServices()
        {
            _tourReservationService = Injector.CreateInstance<ITourReservationService>();
            _checkPointService = Injector.CreateInstance<ICheckPointService>();
            _voucherService = Injector.CreateInstance<ITourVoucherService>();
            _tourDateService = Injector.CreateInstance<ITourDateService>();
            _imageService = Injector.CreateInstance<ITourImageService>();
        }

        private void InitializeCollections()
        {
            TourReservations = new List<TourReservation>();
            Guests = new List<User>();
        }

        private void InitializeCommands()
        {
            OpenGuideOverviewCommand = new RelayCommand(OpenGuideOverview);
            CancelTourCommand = new RelayCommand(CancelTour);
        }

        private void CancelTour(object obj)
        {
            if (SelectedDate == null)
            {
                const string sMessageBoxText = "Select the tour date you want to cancel";
                OnShowMessageBox(sMessageBoxText);
            }
            else
            {
                var timeDifference = CalculateTimeDifference();

                if (timeDifference.TotalHours < 48)
                {
                    const string sMessageBoxText = "You cannot cancel the tour as it is less than 48 hours from now";
                    OnShowMessageBox(sMessageBoxText);
                }
                else
                {
                    var sMessageBoxText = $"Are you sure you want to cancel the tour for {SelectedDate.Date}?";
                    OnShowMessageBoxResult(sMessageBoxText);

                    if (_result != MessageBoxResult.OK) return;
                    GetTourReservations();
                    SendVouchers();
                    DeleteTourData();
                    DisplayCancellationMessage();
                }
            }
        }

        private void DisplayCancellationMessage()
        {
            const string sMessageBoxText = "Tour has successfully been canceled!\n All guests have received a voucher!";
            OnShowMessageBoxResult(sMessageBoxText);

            if (_result != MessageBoxResult.OK) return;
            OnActionCompleted();
        }

        private void DeleteTourData()
        {
            DeleteTourReservations();
            if (Tour.TourDatesViewModel.TourDates.Count == 1)
            {
                const string sMessageBoxText = "Tour data has been completely deleted";
                OnShowMessageBox(sMessageBoxText);

                _checkPointService.DeleteByParentId(Tour.Id);
                _imageService.DeleteAllByParentId(Tour.Id);
            }

            var tourDate = _tourDateService.GetById(SelectedDate.Id);
            _tourDateService.Delete(tourDate);
        }

        private void DeleteTourReservations()
        {
            foreach (var reservation in TourReservations)
            {
                _tourReservationService.Delete(reservation);
            }
        }

        private void SendVouchers()
        {
            Guests = GetGuests();
            var selectedDate = GetSelectedDate();
            var nextYear = selectedDate.AddYears(1);

            foreach (var tourVoucher in Guests.Select(guest => new TourVoucher(guest, nextYear)))
            {
                _voucherService.Save(tourVoucher);
            }
        }

        private List<User> GetGuests()
        {
            return _tourReservationService.GetUsersByTourAndDate(Tour.Id, GetSelectedDate());
        }

        private void GetTourReservations()
        {
            foreach (var tourReservation in _tourReservationService.GetByTourAndDate(Tour.Id, GetSelectedDate()))
            {
                TourReservations.Add(tourReservation);
            }
        }

        private TimeSpan CalculateTimeDifference()
        {
            return _tourDateService.CalculateTimeDifference(SelectedDate.Date);
        }

        private DateTime GetSelectedDate()
        {
            return SelectedDate.Date;
        }

        private void OpenGuideOverview(object obj)
        {
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

    public class ActionCompletedEventArgs : EventArgs
    {

    }
}
