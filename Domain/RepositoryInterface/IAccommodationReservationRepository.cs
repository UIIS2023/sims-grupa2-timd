using System.Collections.Generic;
using SimsProject.Domain.Model;

namespace SimsProject.Domain.RepositoryInterface
{
    public interface IAccommodationReservationRepository : IRepository<AccommodationReservation>
    {
        List<AccommodationReservation> GetByAccommodation(int accommodationId);
        List<AccommodationReservation> GetAllByAccommodation(int accommodationId);
        List<AccommodationReservation> GetByOwner(User owner);
        List<AccommodationReservation> GetCanceledByOwner(User owner);
        List<AccommodationReservation> GetOngoingByOwner(User owner);
        List<AccommodationReservation> GetUpcomingByOwner(User owner);
        List<AccommodationReservation> GetPastByOwner(User owner);
        List<AccommodationReservation> GetUpcomingByGuest(User user);
        List<AccommodationReservation> GetPastByGuest(User user);
        List<AccommodationReservation> GetByGuest(User guest);
    }
}
