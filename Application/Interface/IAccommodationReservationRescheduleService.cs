using SimsProject.Domain.Model;
using System;
using System.Collections.Generic;

namespace SimsProject.Application.Interface
{
    public interface IAccommodationReservationRescheduleService : IService<AccommodationReservationReschedule>
    {
        List<AccommodationReservationReschedule> GetByOwner(User owner);
        List<AccommodationReservationReschedule> GetByGuest(User guest);
        List<AccommodationReservationReschedule> GetByReservation(AccommodationReservation reservation);
        AccommodationReservationReschedule Update(AccommodationReservationReschedule reschedule);
        void CreateRequest(AccommodationReservation selectedReservation, User currentUser, User owner, DateOnly date1, DateOnly date2);
        void AcceptReschedule(int rescheduleId);
        void RejectReschedule(int rescheduleId, string comment);
        bool ShouldNotify(User currentUser);
    }
}
