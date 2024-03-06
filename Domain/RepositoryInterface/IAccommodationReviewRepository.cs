using SimsProject.Domain.Model;
using System.Collections.Generic;

namespace SimsProject.Domain.RepositoryInterface
{
    public interface IAccommodationReviewRepository : IRepository<AccommodationReview>
    {
        List<AccommodationReview> GetByOwner(User owner);
        List<AccommodationReview> GetByGuest(User guest);
        AccommodationReview GetByReservation(AccommodationReservation reservation);
        bool Exists(AccommodationReservation reservation);
    }
}
