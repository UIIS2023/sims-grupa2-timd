using SimsProject.Application.Interface;
using System.Collections.ObjectModel;
using SimsProject.WPF.ViewModel.Domain;
using System.Windows.Input;
using System.Linq;
using SimsProject.Domain.Model;
using System;

namespace SimsProject.WPF.ViewModel.Owner
{
    public class ReservationsPanelViewModel : ViewModelBase
    {
        public User Owner { get; set; }
        public ObservableCollection<AccommodationReservationViewModel> UpcomingReservations { get; set; }
        public ObservableCollection<AccommodationReservationViewModel> PastReservations { get; set; }
        public ObservableCollection<AccommodationReservationRescheduleViewModel> RescheduleRequests { get; set; }

        #region SERVICES

        private IAccommodationReservationService _reservationService;
        private IAccommodationReservationRescheduleService _reservationRescheduleService;

        #endregion

        #region PROPERTIES

        public AccommodationReservationRescheduleViewModel SelectedRequest { get; set; }
        public string Explanation { get; set; }

        #endregion

        #region COMMANDS

        public ICommand AcceptCommand { get; set; }
        public ICommand RejectCommand { get; set; }

        #endregion

        #region EVENTS

        public bool ConfirmActionResult { get; set; }
        public event EventHandler ConfirmActionRequested;
        public bool RejectActionResult { get; set; }
        public event EventHandler RejectActionRequested;

        #endregion

        public ReservationsPanelViewModel(User owner)
        {
            Owner = owner;

            InitializeServices();
            InitializeCollections();
            InitializeCommands();
        }

        private void InitializeServices()
        {
            _reservationService = Injector.CreateInstance<IAccommodationReservationService>();
            _reservationRescheduleService = Injector.CreateInstance<IAccommodationReservationRescheduleService>();
        }

        private void InitializeCollections()
        {
            var upcomingReservations = _reservationService.GetUpcomingByOwner(Owner)
                                                          .Select(reservation => new AccommodationReservationViewModel(reservation))
                                                          .ToList();
            UpcomingReservations = new ObservableCollection<AccommodationReservationViewModel>(upcomingReservations.OrderBy(r => r.ArrivalDate)
                                                                                                                   .ToList());
            var pastReservations = _reservationService.GetPastByOwner(Owner)
                                                      .Select(reservation => new AccommodationReservationViewModel(reservation))
                                                      .ToList();
            PastReservations = new ObservableCollection<AccommodationReservationViewModel>(pastReservations.OrderByDescending(r => r.ArrivalDate)
                                                                                                           .ToList());

            RescheduleRequests = new ObservableCollection<AccommodationReservationRescheduleViewModel>();
            foreach (var request in _reservationRescheduleService.GetByOwner(Owner))
            {
                RescheduleRequests.Add(new AccommodationReservationRescheduleViewModel(request));
            }
        }

        private void InitializeCommands()
        {
            AcceptCommand = new RelayCommand(Accept);
            RejectCommand = new RelayCommand(Reject);
        }

        private void Accept(object obj)
        {
            ConfirmActionRequested?.Invoke(this, EventArgs.Empty);
            if (!ConfirmActionResult) return;

            _reservationRescheduleService.AcceptReschedule(SelectedRequest.Id);
            RescheduleRequests.Remove(SelectedRequest);
            UpdateUpcomingReservations();
        }

        private void Reject(object obj)
        {
            RejectActionRequested?.Invoke(this, EventArgs.Empty);
            if (!RejectActionResult) return;

            _reservationRescheduleService.RejectReschedule(SelectedRequest.Id, Explanation);
            RescheduleRequests.Remove(SelectedRequest);
        }

        private void UpdateUpcomingReservations()
        {
            UpcomingReservations.Clear();
            foreach (var reservation in _reservationService.GetUpcomingByOwner(Owner))
            {
                UpcomingReservations.Add(new AccommodationReservationViewModel(reservation));
            }
        }
    }
}