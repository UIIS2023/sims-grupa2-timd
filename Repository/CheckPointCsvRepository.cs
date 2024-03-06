using SimsProject.Domain.Model;
using SimsProject.Serializer;
using System.Collections.Generic;
using System.Linq;
using SimsProject.Domain.RepositoryInterface;

namespace SimsProject.Repository
{
    public class CheckPointCsvRepository : ICheckPointRepository
    {
        private const string FilePath = "../../../Resources/Data/checkPoints.csv";

        private readonly Serializer<CheckPoint> _serializer;
        private List<CheckPoint> _checkPoints;

        public CheckPointCsvRepository()
        {
            _serializer = new Serializer<CheckPoint>();
            _checkPoints = _serializer.FromCsv(FilePath);
        }

        public List<CheckPoint> GetAll()
        {
            var checkPoints = _serializer.FromCsv(FilePath);
            return checkPoints;
        }

        public CheckPoint GetById(int id)
        {
            _checkPoints = _serializer.FromCsv(FilePath);
            return _checkPoints.FirstOrDefault(l => l.Id == id);
        }

        public int NextId()
        {
            _checkPoints = GetAll();
            if (_checkPoints.Count < 1)
            {
                return 1;
            }
            return _checkPoints.Max(c => c.Id) + 1;
        }

        public CheckPoint Save(CheckPoint checkPoint)
        {
            checkPoint.Id = NextId();
            _checkPoints = GetAll();
            _checkPoints.Add(checkPoint);
            _serializer.ToCsv(FilePath, _checkPoints);

            return checkPoint;
        }

        public CheckPoint Update(CheckPoint checkPoint)
        {
            _checkPoints = _serializer.FromCsv(FilePath);
            var current = _checkPoints.Find(u => u.Id == checkPoint.Id);
            var index = _checkPoints.IndexOf(current);
            _checkPoints.Remove(current);
            _checkPoints.Insert(index, checkPoint);
            _serializer.ToCsv(FilePath, _checkPoints);
            return checkPoint;
        }

        public void Delete(CheckPoint checkPoint)
        {
            _checkPoints = _serializer.FromCsv(FilePath);
            var found = _checkPoints.Find(a => a.Id == checkPoint.Id);
            _checkPoints.Remove(found);
            _serializer.ToCsv(FilePath, _checkPoints);
        }

        public List<CheckPoint> GetByParentId(int parentId)
        {
            _checkPoints = GetAll();
            return _checkPoints.FindAll(i => i.Tour.Id == parentId);
        }

        public void DeleteByParentId(int parentId)
        {
            _checkPoints = _serializer.FromCsv(FilePath);
            var found = _checkPoints.FindAll(i => i.Tour.Id == parentId);
            foreach (var checkPoint in found)
            {
                _checkPoints.Remove(checkPoint);
            }
            _serializer.ToCsv(FilePath, _checkPoints);
        }

        public CheckPoint UpdateByTourId(int checkPointId, int tourId)
        {
            _checkPoints = GetAll();
            int index;
            var oldCheckPoint = _checkPoints.Find(c => c.IsActive && c.Tour.Id == tourId);
            if (oldCheckPoint != null)
            {
                oldCheckPoint.IsActive = false;
                index = _checkPoints.IndexOf(oldCheckPoint);
                _checkPoints.Remove(oldCheckPoint);
                _checkPoints.Insert(index, oldCheckPoint);
            }

            var currentCheckPoint = _checkPoints.Find(c => c.Id == checkPointId && c.Tour.Id == tourId);
            currentCheckPoint.IsActive = true;
            index = _checkPoints.IndexOf(currentCheckPoint);
            _checkPoints.Remove(currentCheckPoint);
            _checkPoints.Insert(index, currentCheckPoint);
            _serializer.ToCsv(FilePath, _checkPoints);

            return currentCheckPoint;
        }

        public void SetDefault()
        {
            foreach (var checkPoint in _checkPoints)
            {
                checkPoint.IsActive = false;
                _serializer.ToCsv(FilePath, _checkPoints);
            }
        }

        internal CheckPoint Get(int id)
        {
            _checkPoints = GetAll();
            return _checkPoints.FirstOrDefault(i => i.Id == id);
        }
    }
}
