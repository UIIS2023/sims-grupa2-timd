using SimsProject.Serializer;
using System.Collections.Generic;
using System.Linq;
using SimsProject.Domain.Model;
using SimsProject.Domain.RepositoryInterface;
using User = SimsProject.Domain.Model.User;

namespace SimsProject.Repository
{
    public class TourCsvRepository : ITourRepository
    {
        private const string FilePath = "../../../Resources/Data/tours.csv";

        private readonly Serializer<Tour> _serializer;
        private List<Tour> _tours;

        public TourCsvRepository()
        {
            _serializer = new Serializer<Tour>();
            _tours = _serializer.FromCsv(FilePath);
        }

        public List<Tour> GetAll()
        {
            return _serializer.FromCsv(FilePath);
        }

        public Tour GetById(int id)
        {
            _tours = _serializer.FromCsv(FilePath);
            return _tours.FirstOrDefault(a => a.Id == id);
        }

        public int NextId()
        {
            _tours = _serializer.FromCsv(FilePath);
            if (_tours.Count < 1)
            {
                return 1;
            }
            return _tours.Max(t => t.Id) + 1;
        }

        public Tour Save(Tour tour)
        {
            tour.Id = NextId();
            _tours = _serializer.FromCsv(FilePath);
            _tours.Add(tour);
            _serializer.ToCsv(FilePath, _tours);
            return tour;
        }

        public List<Tour> GetByUser(User user)
        {
            _tours = _serializer.FromCsv(FilePath);
            return _tours.FindAll(t => t.Guide.Id == user.Id);
        }

        public Tour Update(Tour tour)
        {
            _tours = _serializer.FromCsv(FilePath);
            var current = _tours.Find(t => t.Id == tour.Id);
            var index = _tours.IndexOf(current);
            _tours.Remove(current);
            _tours.Insert(index, tour);
            _serializer.ToCsv(FilePath, _tours);

            return tour;
        }

        public void Delete(Tour tour)
        {
            _tours = _serializer.FromCsv(FilePath);
            var found = _tours.Find(a => a.Id == tour.Id);
            _tours.Remove(found);
            _serializer.ToCsv(FilePath, _tours);
        }

        public void DeleteById(int tourId)
        {
            _tours = _serializer.FromCsv(FilePath);
            var found = _tours.Find(a => a.Id == tourId);
            _tours.Remove(found);
            _serializer.ToCsv(FilePath, _tours);
        }

        public Tour GetByParentId(int parentId)
        {
            _tours = GetAll();
            return _tours.Find(t => t.Id == parentId);
        }
        
        public List<Tour> GetByGuide(User owner)
        {
            _tours = _serializer.FromCsv(FilePath);
            return _tours.FindAll(a => a.Guide.Id == owner.Id);
        }
    }
}