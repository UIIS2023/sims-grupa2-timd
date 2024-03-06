using SimsProject.Domain.Model;
using System.Collections.Generic;

namespace SimsProject.Domain.RepositoryInterface
{
    public interface ITourReviewRepository : IRepository<TourReview>
    {
        TourReview UpdateById(int tourReviewId);
        void DeleteAllByParentId(int parentId);
        List<TourReview> GetByGuest(User guest);
        List<TourReview> GetByTour(Tour tour);
    }
}
