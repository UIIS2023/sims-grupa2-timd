using SimsProject.Domain.Model;
using System.Collections.Generic;

namespace SimsProject.Domain.RepositoryInterface
{
    public interface ITourDateRepository : IRepository<TourDate>
    {
        List<TourDate> GetByParentId(int parentId);
        void DeleteAllByParentId(int parentId);
        TourDate UpdateById(int tourDateId);
        List<string> FindDistinctYears();
    }
}
