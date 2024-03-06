using SimsProject.Domain.Model;
using SimsProject.Serializer;
using System.Collections.Generic;
using System.Linq;
using SimsProject.Domain.RepositoryInterface;

namespace SimsProject.Repository
{
    public class AccommodationTypeCsvRepository : IAccommodationTypeRepository
    {
        private const string FilePath = "../../../Resources/Data/accommodationTypes.csv";

        private readonly Serializer<AccommodationType> _serializer;

        private List<AccommodationType> _accommodationTypes;

        public AccommodationTypeCsvRepository()
        {
            _serializer = new Serializer<AccommodationType>();
            _accommodationTypes = _serializer.FromCsv(FilePath);
        }

        public List<AccommodationType> GetAll()
        {
            return _serializer.FromCsv(FilePath);
        }

        public AccommodationType GetById(int id)
        {
            _accommodationTypes = _serializer.FromCsv(FilePath);
            return _accommodationTypes.FirstOrDefault(t => t.Id == id);
        }

        public int NextId()
        {
            _accommodationTypes = _serializer.FromCsv(FilePath);
            if (_accommodationTypes.Count < 1)
            {
                return 1;
            }
            return _accommodationTypes.Max(t => t.Id) + 1;
        }

        public AccommodationType Save(AccommodationType type)
        {
            type.Id = NextId();
            _accommodationTypes = _serializer.FromCsv(FilePath);
            _accommodationTypes.Add(type);
            _serializer.ToCsv(FilePath, _accommodationTypes);
            return type;
        }

        public AccommodationType Update(AccommodationType type)
        {
            _accommodationTypes = _serializer.FromCsv(FilePath);
            AccommodationType current = _accommodationTypes.Find(t => t.Id == type.Id);
            int index = _accommodationTypes.IndexOf(current);
            _accommodationTypes.Remove(current);
            _accommodationTypes.Insert(index, type);
            _serializer.ToCsv(FilePath, _accommodationTypes);
            return type;
        }

        public void Delete(AccommodationType type)
        {
            _accommodationTypes = _serializer.FromCsv(FilePath);
            AccommodationType found = _accommodationTypes.Find(t => t.Id == type.Id);
            _accommodationTypes.Remove(found);
            _serializer.ToCsv(FilePath, _accommodationTypes);
        }
    }
}
