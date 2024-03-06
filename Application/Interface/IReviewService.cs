using SimsProject.Domain.Model;
using System.Collections.Generic;

namespace SimsProject.Application.Interface
{
    public interface IReviewService
    { 
        List<GuestReview> GetReviewsByGuest(User guest);
        List<AccommodationReview> GetGuestReviewedByOwner(User owner);
        List<AccommodationReservation> GetUnreviewedByOwner(User owner);
        List<AccommodationReservation> GetUnreviewedByGuest(User guest);
    }
}
