using System.Collections.Generic;
using SimsProject.Domain.Model;

namespace SimsProject.Domain.RepositoryInterface
{
    public interface IAccommodationReservationRescheduleRepository : IRepository<AccommodationReservationReschedule>
    {
        List<AccommodationReservationReschedule> GetByGuest(User guest);
        List<AccommodationReservationReschedule> GetByOwner(User owner);
        List<AccommodationReservationReschedule> GetByReservation(AccommodationReservation reservation);
        bool NewNotificationExists(User user);
    }
}
