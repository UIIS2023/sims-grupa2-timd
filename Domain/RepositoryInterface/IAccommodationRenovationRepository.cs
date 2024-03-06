using SimsProject.Domain.Model;
using System.Collections.Generic;

namespace SimsProject.Domain.RepositoryInterface
{
    public interface IAccommodationRenovationRepository : IRepository<AccommodationRenovation>
    {
        List<AccommodationRenovation> GetByAccommodation(int accommodationId);
        List<AccommodationRenovation> GetByOwner(User owner);
        List<AccommodationRenovation> GetOngoingByOwner(User owner);
        List<AccommodationRenovation> GetUpcomingByOwner(User owner);
        List<AccommodationRenovation> GetPastByOwner(User owner);
    }
}