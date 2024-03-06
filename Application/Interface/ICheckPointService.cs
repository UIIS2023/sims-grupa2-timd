using SimsProject.Domain.Model;
using System.Collections.Generic;

namespace SimsProject.Application.Interface
{
    public interface ICheckPointService : IService<CheckPoint>
    {
        List<CheckPoint> GetByParentId(int parentId);
        List<CheckPoint> GetAll();
        void DeleteByParentId(int parentId);
        CheckPoint Save(CheckPoint checkPoint);
        CheckPoint UpdateByTourId(int checkPointId, int tourId);
        void SetDefault();
    }
}
