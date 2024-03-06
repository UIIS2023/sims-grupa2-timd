using SimsProject.Domain.Model;
using System;
using SimsProject.Application.Interface;
using SimsProject.Domain.RepositoryInterface;
using System.Collections.Generic;
using System.Linq;

namespace SimsProject.Application.UseCase
{
    public class AccommodationReservationRescheduleService : IAccommodationReservationRescheduleService
    {
        private readonly IAccommodationReservationRescheduleRepository _rescheduleRepository;
        private readonly IAccommodationReservationService _reservationService;
        private readonly IAccommodationRenovationService _renovationService;

        public AccommodationReservationRescheduleService()
        {
            _rescheduleRepository = Injector.CreateInstance<IAccommodationReservationRescheduleRepository>();
            _reservationService = Injector.CreateInstance<IAccommodationReservationService>();
            _renovationService = Injector.CreateInstance<IAccommodationRenovationService>();
        }

        public AccommodationReservationReschedule GetById(int id)
        {
            return _rescheduleRepository.GetById(id);
        }

        public List<AccommodationReservationReschedule> GetByOwner(User owner)
        {
            var requests = _rescheduleRepository.GetByOwner(owner)
                .Where(request => request.RequestStatus == Status.Waiting)
                .ToList();

            UpdateNewDatesAvailability(requests);

            return requests;
        }

        public List<AccommodationReservationReschedule> GetByReservation(AccommodationReservation reservation)
        {
            return _rescheduleRepository.GetByReservation(reservation);
        }

        public AccommodationReservationReschedule Update(AccommodationReservationReschedule reschedule)
        {
            return _rescheduleRepository.Update(reschedule);
        }

        private void UpdateNewDatesAvailability(List<AccommodationReservationReschedule> requests)
        {
            foreach (var request in requests.Where(request => request.NewDatesAvailable != AreDatesAvailable(request)))
            {
                request.NewDatesAvailable = AreDatesAvailable(request);
                _rescheduleRepository.Update(request);
            }
        }

        private bool AreDatesAvailable(AccommodationReservationReschedule request)
        {
            var reservationToReschedule = _reservationService.GetById(request.Reservation.Id);

            if (_renovationService.GetByAccommodation(reservationToReschedule.Accommodation.Id)
                .Any(r => r.StartDate < request.NewCheckoutDate && r.EndDate > request.NewArrivalDate)) return false;

            var allReservations = _reservationService.GetByOwner(request.Owner)
                                .Where(r => !r.IsCanceled && r.Id != reservationToReschedule.Id);

            return allReservations.All(r => r.ArrivalDate >= request.NewCheckoutDate || r.CheckoutDate <= request.NewArrivalDate);
        }

        public void AcceptReschedule(int rescheduleId)
        {
            var request = _rescheduleRepository.GetById(rescheduleId);

            DeleteOverlappingReservations(request);
            UpdateRescheduledReservation(request);
            UpdateRequestStatus(request, true);
        }

        private void DeleteOverlappingReservations(AccommodationReservationReschedule request)
        {
            foreach (var reservation in _reservationService.GetByOwner(request.Owner))
            {
                if (reservation.Id == request.Reservation.Id) continue;
            
                if (reservation.ArrivalDate < request.NewCheckoutDate && reservation.CheckoutDate > request.NewArrivalDate)
                {
                    _reservationService.Delete(reservation);
                }
            }
        }

        private void UpdateRescheduledReservation(AccommodationReservationReschedule request)
        {
            var rescheduledReservation = _reservationService.GetById(request.Reservation.Id);
            rescheduledReservation.ArrivalDate = request.NewArrivalDate;
            rescheduledReservation.CheckoutDate = request.NewCheckoutDate;
            rescheduledReservation.StayLength = (request.NewCheckoutDate.ToDateTime(TimeOnly.MinValue) - request.NewArrivalDate.ToDateTime(TimeOnly.MinValue)).Days + 1;

            _reservationService.Update(rescheduledReservation);
        }

        public void RejectReschedule(int rescheduleId, string comment)
        {
            var request = _rescheduleRepository.GetById(rescheduleId);
            request.Comment = comment;

            UpdateRequestStatus(request, false);
        }

        private void UpdateRequestStatus(AccommodationReservationReschedule request, bool accepted)
        {
            request.RequestStatus = accepted ? Status.Accepted : Status.Declined;
            request.NotifyUserOfStatusChange = true;

            _rescheduleRepository.Update(request);
        }

        public List<AccommodationReservationReschedule> GetByGuest(User guest)
        {
            List<AccommodationReservationReschedule> requests = new(_rescheduleRepository.GetByGuest(guest));
            return requests;
        }

        public void CreateRequest(AccommodationReservation selectedReservation, User currentUser, User owner, DateOnly date1, DateOnly date2)
        {
            var reschedule = new AccommodationReservationReschedule(selectedReservation, currentUser, owner, date1, date2, Status.Waiting, "");
            _rescheduleRepository.Save(reschedule);
        }

        public bool ShouldNotify(User currentUser)
        {
            if (_rescheduleRepository.NewNotificationExists(currentUser))
            {
                var list = _rescheduleRepository.GetByGuest(currentUser);
                foreach (var l in list)
                {
                    l.NotifyUserOfStatusChange = false;
                    _rescheduleRepository.Update(l);
                }
                return true;
            }
            return false;
        }
    }
}
