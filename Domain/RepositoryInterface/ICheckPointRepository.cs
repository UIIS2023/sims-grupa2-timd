using SimsProject.Domain.Model;
using System.Collections.Generic;

namespace SimsProject.Domain.RepositoryInterface
{
    public interface ICheckPointRepository : IRepository<CheckPoint>
    {
        List<CheckPoint> GetByParentId(int parentId);
        void DeleteByParentId(int parentId);
        CheckPoint UpdateByTourId(int checkPointId, int tourId);
        void SetDefault();
    }
}
