using System;
using SimsProject.Application.Interface;
using SimsProject.Domain.Model;

namespace SimsProject.WPF.ViewModel.Domain
{
    public class TourReservationViewModel : ViewModelBase
    {
        private readonly ITourReservationService _tourReservationService;
        private readonly TourReservation _tourReservation;

        public TourViewModel Tour { get; }
        public UserViewModel Guest { get; }
        public int GuestNumber => _tourReservation.GuestNumber;
        public DateTime Date => _tourReservation.Date;
        public int GuestAge => _tourReservation.GuestAge;

        public TourReservationViewModel(TourReservation tourReservation)
        {
            _tourReservationService = Injector.CreateInstance<ITourReservationService>();
            _tourReservation = _tourReservationService.GetById(tourReservation.Id);

            Tour = new TourViewModel(_tourReservation.Tour);
            Guest = new UserViewModel(_tourReservation.Guest);
        }
    }
}
