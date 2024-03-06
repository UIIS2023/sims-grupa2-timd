using SimsProject.Domain.Model;
using SimsProject.Domain.RepositoryInterface;
using SimsProject.Serializer;
using System.Collections.Generic;
using System.Linq;

namespace SimsProject.Repository
{
    public class TourDateCsvRepository : ITourDateRepository
    {

        private const string FilePath = "../../../Resources/Data/tourDates.csv";

        private readonly Serializer<TourDate> _serializer;
        private List<TourDate> _tourDates;

        public TourDateCsvRepository()
        {
            _serializer = new Serializer<TourDate>();
            _tourDates = _serializer.FromCsv(FilePath);
        }

        public List<TourDate> GetAll()
        {
            var tourDates = _serializer.FromCsv(FilePath);
            return tourDates;
        }

        public TourDate GetById(int id)
        {
            _tourDates = _serializer.FromCsv(FilePath);
            return _tourDates.FirstOrDefault(l => l.Id == id);
        }

        public int NextId()
        {
            _tourDates = GetAll();
            if (_tourDates.Count < 1)
            {
                return 1;
            }
            return _tourDates.Max(t => t.Id) + 1;
        }

        public TourDate Save(TourDate tourDate)
        {
            tourDate.Id = NextId();
            _tourDates = GetAll();
            _tourDates.Add(tourDate);
            _serializer.ToCsv(FilePath, _tourDates);

            return tourDate;
        }

        public void Delete(TourDate tourDate)
        {
            _tourDates = _serializer.FromCsv(FilePath);
            var found = _tourDates.Find(a => a.Id == tourDate.Id);
            _tourDates.Remove(found);
            _serializer.ToCsv(FilePath, _tourDates);
        }

        public TourDate Update(TourDate tourDate)
        {
            _tourDates = GetAll();
            var currentDate = _tourDates.Find(d => d.Id == tourDate.Id);

            currentDate.HasEnded = tourDate.HasEnded;
            var index = _tourDates.IndexOf(currentDate);

            _tourDates.Remove(currentDate);
            _tourDates.Insert(index, currentDate);
            _serializer.ToCsv(FilePath, _tourDates);

            return tourDate;
        }

        public List<TourDate> GetByParentId(int parentId)
        {
            _tourDates = GetAll();
            return _tourDates.FindAll(t => t.Tour.Id == parentId);
        }

        public void DeleteAllByParentId(int parentId)
        {
            _tourDates = _serializer.FromCsv(FilePath);
            var found = _tourDates.FindAll(i => i.Tour.Id == parentId);
            foreach (var checkPoint in found)
            {
                _tourDates.Remove(checkPoint);
            }
            _serializer.ToCsv(FilePath, _tourDates);
        }

        public TourDate UpdateById(int tourDateId)
        {
            _tourDates = GetAll();
            var tourDate = _tourDates.Find(d => d.Id == tourDateId);
            tourDate.HasEnded = true;
            var index = _tourDates.IndexOf(tourDate);

            _tourDates.Remove(tourDate);
            _tourDates.Insert(index, tourDate);
            _serializer.ToCsv(FilePath, _tourDates);
            return tourDate;
        }

        public List<string> FindDistinctYears()
        {
            var years = new List<string>();
            foreach (var date in _tourDates.Where(date => !years.Contains(date.Date.Year.ToString())))
            {
                years.Add(date.Date.Year.ToString());
            }
            return years;
        }
    }
}
