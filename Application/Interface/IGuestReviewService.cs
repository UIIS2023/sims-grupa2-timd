using SimsProject.Domain.Model;
using System.Collections.Generic;

namespace SimsProject.Application.Interface
{
    public interface IGuestReviewService : IService<GuestReview>
    {
        List<GuestReview> GetByGuest(User guest);
        bool Exists(AccommodationReservation reservation);
        GuestReview CreateReview(GuestReview guestReview);
    }
}
