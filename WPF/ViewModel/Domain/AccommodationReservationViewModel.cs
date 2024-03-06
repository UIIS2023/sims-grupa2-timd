using System;
using SimsProject.Application.Interface;
using SimsProject.Domain.Model;

namespace SimsProject.WPF.ViewModel.Domain
{
    public class AccommodationReservationViewModel : ViewModelBase
    {
        private readonly AccommodationReservation _reservation;

        internal int Id => _reservation.Id;
        public AccommodationViewModel Accommodation { get; }
        public User Guest { get; }
        public DateOnly ArrivalDate { get; set; }
        public DateOnly CheckoutDate { get; set; }
        public int StayLength => _reservation.StayLength;
        public int GuestNumber => _reservation.GuestNumber;
        public bool IsCanceled => _reservation.IsCanceled;
        public bool NotifyOwner => _reservation.NotifyOwnerOfCancel;

        public AccommodationReservationViewModel(AccommodationReservation reservation)
        {
            var reservationService = Injector.CreateInstance<IAccommodationReservationService>();
            var userService = Injector.CreateInstance<IUserService>();
            _reservation = reservationService.GetById(reservation.Id);

            Accommodation = new AccommodationViewModel(_reservation.Accommodation);
            Guest = userService.GetById(_reservation.Guest.Id);
            ArrivalDate = _reservation.ArrivalDate;
            CheckoutDate = _reservation.CheckoutDate;
        }
    }
}
