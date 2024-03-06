using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using System;

namespace SimsProject.WPF.ViewModel.Domain
{
    public class AccommodationReservationRescheduleViewModel : ViewModelBase
    {
        private readonly AccommodationReservationReschedule _reservationReschedule;

        internal int Id => _reservationReschedule.Id;
        public AccommodationReservationViewModel Reservation { get; }
        public User Guest { get; }
        public User Owner { get; }
        public DateOnly NewArrivalDate { get; set; }
        public DateOnly NewCheckoutDate { get; set; }
        public bool NewDatesAvailable => _reservationReschedule.NewDatesAvailable;
        public Status RequestStatus => _reservationReschedule.RequestStatus;
        public string Comment => string.IsNullOrEmpty(_reservationReschedule.Comment) ? "No comment" : _reservationReschedule.Comment;

        public bool Notify => _reservationReschedule.NotifyUserOfStatusChange;

        public AccommodationReservationRescheduleViewModel(AccommodationReservationReschedule reschedule)
        {
            var accommodationRescheduleService = Injector.CreateInstance<IAccommodationReservationRescheduleService>();
            var userService = Injector.CreateInstance<IUserService>();
            _reservationReschedule = accommodationRescheduleService.GetById(reschedule.Id);

            Reservation = new AccommodationReservationViewModel(_reservationReschedule.Reservation);
            Guest = userService.GetById(_reservationReschedule.Guest.Id);
            Owner = userService.GetById(_reservationReschedule.Owner.Id);
            NewArrivalDate = _reservationReschedule.NewArrivalDate;
            NewCheckoutDate = _reservationReschedule.NewCheckoutDate;
        }
    }
}
