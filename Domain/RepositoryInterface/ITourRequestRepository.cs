using SimsProject.Domain.Model;
using System.Collections.Generic;

namespace SimsProject.Domain.RepositoryInterface
{
    public interface ITourRequestRepository : IRepository<TourRequest>
    {
        List<TourRequest> GetByGuide(User guide);
        TourRequest UpdateById(int tourRequestId);
        List<TourRequest> GetByParentId(int parentId);
    }
}
