using System.Collections.Generic;
using SimsProject.Domain.Model;
using SimsProject.Domain.RepositoryInterface;
using SimsProject.Application.Interface;

namespace SimsProject.Application.UseCase
{
    public class CheckPointService : ICheckPointService
    {
        private readonly ICheckPointRepository _checkPointRepository;

        public CheckPointService()
        {
            _checkPointRepository = Injector.CreateInstance<ICheckPointRepository>();
        }

        public List<CheckPoint> GetAll()
        {
            return _checkPointRepository.GetAll();
        }

        public CheckPoint GetById(int id)
        {
            return _checkPointRepository.GetById(id);
        }

        public List<CheckPoint> GetByParentId(int parentId) 
        {
            return _checkPointRepository.GetByParentId(parentId);
        }

        public void DeleteByParentId(int parentId)
        {
            _checkPointRepository.DeleteByParentId(parentId);
        }

        public CheckPoint Save(CheckPoint checkPoint)
        {
            return _checkPointRepository.Save(checkPoint);
        }

        public CheckPoint UpdateByTourId(int checkPointId, int tourId)
        {
            return _checkPointRepository.UpdateByTourId(checkPointId, tourId);
        }

        public void SetDefault()
        {
            _checkPointRepository.SetDefault();
        }
    }
}
