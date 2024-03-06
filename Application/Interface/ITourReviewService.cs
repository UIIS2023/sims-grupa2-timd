using SimsProject.Domain.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SimsProject.Application.Interface
{
    public interface ITourReviewService : IService<TourReview>
    {
        List<TourReview> GetAll();
        void DeleteAllByParentId(int parentId);
        void CreateTourReview(User loggedInUser, int knowledge, int language, int interestigness, string comment, List<TourAttendance> attendances, TourReservation reservation, ObservableCollection<Image> images, TourDate date);
        TourReview Update(TourReview tourReview);
        TourReview UpdateById(int tourReviewId);
        List<TourReview> GetByGuest(User guest);
        List<TourReview> GetFilteredReviews(int tourId);
        bool ExistsReview(TourReservation reservation);
        List<TourReview> GetByTour(Tour tour);
    }
}
