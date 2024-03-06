using SimsProject.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimsProject.Domain.RepositoryInterface
{
    public interface IGuestReviewRepository : IRepository<GuestReview>
    {
        bool Exists(AccommodationReservation reservation);
        List<GuestReview> GetByGuest(User guest);
        List<GuestReview> GetByOwner(User owner);
    }
}
