using SimsProject.Domain.Model;
using System.Collections.Generic;

namespace SimsProject.Domain.RepositoryInterface
{
    public interface ITourRepository : IRepository<Tour>
    {
        List<Tour> GetByGuide(User guide);
        void DeleteById(int tourId);
        Tour GetByParentId(int parentId);
    }
}
