using SimsProject.Domain.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SimsProject.Application.Interface
{
    public interface IAccommodationReviewService : IService<AccommodationReview>
    {
        List<AccommodationReview> GetByOwner(User owner);
        AccommodationReview GetByReservation(AccommodationReservation reservation);
        bool Exists(AccommodationReservation reservation);
        AccommodationReview CreateReview(AccommodationReview review, int id, ObservableCollection<Image> images);
    }
}
