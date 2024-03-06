using System;
using System.Collections.Generic;
using System.Linq;
using SimsProject.Domain.Model;
using SimsProject.Domain.RepositoryInterface;
using SimsProject.Serializer;

namespace SimsProject.Repository
{
    public class AccommodationRenovationCsvRepository : IAccommodationRenovationRepository
    {
        private const string FilePath = "../../../Resources/Data/accommodationRenovations.csv";

        private readonly Serializer<AccommodationRenovation> _serializer;

        private List<AccommodationRenovation> _renovations;

        public AccommodationRenovationCsvRepository()
        {
            _serializer = new Serializer<AccommodationRenovation>();
            _renovations = _serializer.FromCsv(FilePath);
        }

        public List<AccommodationRenovation> GetAll()
        {
            return _serializer.FromCsv(FilePath);
        }

        public AccommodationRenovation GetById(int id)
        {
            _renovations = _serializer.FromCsv(FilePath);
            return _renovations.FirstOrDefault(r => r.Id == id);
        }

        public int NextId()
        {
            _renovations = _serializer.FromCsv(FilePath);
            if (_renovations.Count < 1)
            {
                return 1;
            }
            return _renovations.Max(r => r.Id) + 1;
        }

        public AccommodationRenovation Save(AccommodationRenovation renovation)
        {
            renovation.Id = NextId();
            _renovations = _serializer.FromCsv(FilePath);
            _renovations.Add(renovation);
            _serializer.ToCsv(FilePath, _renovations);
            return renovation;
        }

        public AccommodationRenovation Update(AccommodationRenovation renovation)
        {
            _renovations = _serializer.FromCsv(FilePath);
            AccommodationRenovation current = _renovations.Find(r => r.Id == renovation.Id);
            int index = _renovations.IndexOf(current);
            _renovations.Remove(current);
            _renovations.Insert(index, renovation);
            _serializer.ToCsv(FilePath, _renovations);
            return renovation;
        }

        public void Delete(AccommodationRenovation renovation)
        {
            _renovations = _serializer.FromCsv(FilePath);
            AccommodationRenovation found = _renovations.Find(r => r.Id == renovation.Id);
            _renovations.Remove(found);
            _serializer.ToCsv(FilePath, _renovations);
        }

        public List<AccommodationRenovation> GetByAccommodation(int accommodationId)
        {
            _renovations = _serializer.FromCsv(FilePath);
            return _renovations.FindAll(r => r.Accommodation.Id == accommodationId);
        }

        public List<AccommodationRenovation> GetByOwner(User owner)
        {
            _renovations = _serializer.FromCsv(FilePath);
            return _renovations.FindAll(r => r.Owner.Id == owner.Id);
        }

        public List<AccommodationRenovation> GetOngoingByOwner(User owner)
        {
            return (from renovation in _renovations.FindAll(a => a.Owner.Id == owner.Id)
                    let hasStarted = renovation.StartDate.CompareTo(DateOnly.FromDateTime(DateTime.Today)) <= 0
                    let hasPassed = renovation.EndDate.CompareTo(DateOnly.FromDateTime(DateTime.Today)) <= 0
                    where hasStarted && !hasPassed
                    select renovation).ToList();
        }

        public List<AccommodationRenovation> GetUpcomingByOwner(User owner)
        {
            return (from renovation in _renovations.FindAll(a => a.Owner.Id == owner.Id)
                    let hasStarted = renovation.StartDate.CompareTo(DateOnly.FromDateTime(DateTime.Today)) <= 0
                    where !hasStarted
                    select renovation).ToList();
        }

        public List<AccommodationRenovation> GetPastByOwner(User owner)
        {
            return (from renovation in _renovations.FindAll(a => a.Owner.Id == owner.Id)
                    let hasPassed = renovation.EndDate.CompareTo(DateOnly.FromDateTime(DateTime.Today)) <= 0
                    where hasPassed
                    select renovation).ToList();
        }
    }
}