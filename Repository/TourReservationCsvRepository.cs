using SimsProject.Domain.Model;
using SimsProject.Domain.RepositoryInterface;
using SimsProject.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimsProject.Repository
{
    public class TourReservationCsvRepository : ITourReservationRepository
    {
        private const string FilePath = "../../../Resources/Data/tourReservationss.csv";

        private readonly Serializer<TourReservation> _serializer;

        private List<TourReservation> _tourReservations;

        public TourReservationCsvRepository()
        {
            _serializer = new Serializer<TourReservation>();
            _tourReservations = _serializer.FromCsv(FilePath);
        }

        public List<TourReservation> GetAll()
        {
            return _serializer.FromCsv(FilePath);
        }

        public TourReservation GetById(int id)
        {
            _tourReservations = _serializer.FromCsv(FilePath);
            return _tourReservations.FirstOrDefault(a => a.Id == id);
        }

        public TourReservation Save(TourReservation tourReservation)
        {
            tourReservation.Id = NextId();
            _tourReservations = _serializer.FromCsv(FilePath);
            _tourReservations.Add(tourReservation);
            _serializer.ToCsv(FilePath, _tourReservations);
            return tourReservation;
        }

        public TourReservation Update(TourReservation tourReservation)
        {
            _tourReservations = _serializer.FromCsv(FilePath);
            TourReservation current = _tourReservations.Find(a => a.Id == tourReservation.Id);
            int index = _tourReservations.IndexOf(current);
            _tourReservations.Remove(current);
            _tourReservations.Insert(index, tourReservation);
            _serializer.ToCsv(FilePath, _tourReservations);
            return tourReservation;
        }

        public int NextId()
        {
            _tourReservations = _serializer.FromCsv(FilePath);
            if (_tourReservations.Count < 1)
            {
                return 1;
            }
            return _tourReservations.Max(c => c.Id) + 1;
        }

        public void Delete(TourReservation tourReservation)
        {
            _tourReservations = _serializer.FromCsv(FilePath);
            TourReservation founded = _tourReservations.Find(c => c.Id == tourReservation.Id);
            _tourReservations.Remove(founded);
            _serializer.ToCsv(FilePath, _tourReservations);
        }

        public List<User> GetUsersByTourAndDate(int tourId, DateTime tourDate)
        {
            _tourReservations = _serializer.FromCsv(FilePath);
            List<User> users = new List<User>();
            foreach (var tourReservation in _tourReservations)
            {
                if (tourReservation.Tour.Id == tourId && tourReservation.Date == tourDate)
                {
                    users.Add(tourReservation.Guest);
                }
            }
            return users;
        }

        public List<TourReservation> GetByTourAndDate(int tourId, DateTime tourDate)
        {
            _tourReservations = _serializer.FromCsv(FilePath);
            var tourReservations = new List<TourReservation>();
            foreach (var tourReservation in _tourReservations)
            {
                if (tourReservation.Tour.Id == tourId && tourReservation.Date == tourDate)
                {
                    tourReservations.Add(tourReservation);
                }
            }
            return tourReservations;
        }

        public List<Tour> GetReservedToursByUser(User user, List<Tour> allTours)
        {
            _tourReservations = _serializer.FromCsv(FilePath);
            var tours = new List<Tour>();
            foreach (var tour in allTours)
            {
                foreach (var tourReservation in _tourReservations)
                {
                    if (tourReservation.Guest.Id == user.Id && tour.Id == tourReservation.Tour.Id)
                    {
                        if (!tours.Contains(tour)) {
                            tours.Add(tour);
                        }
                    }
                }
            }
            return tours;
        }

        public List<TourReservation> GetByTour(Tour tour)
        {
            _tourReservations = _serializer.FromCsv(FilePath);
            return _tourReservations.FindAll(a => a.Tour.Id == tour.Id);
        }

        public List<TourReservation> GetByGuide(User guide)
        {
            _tourReservations = _serializer.FromCsv(FilePath);
            return _tourReservations.FindAll(a => a.Guide.Id == guide.Id);
        }

        public void DeleteAllByParentId(int parentId)
        {
            _tourReservations = _serializer.FromCsv(FilePath);
            var found = _tourReservations.FindAll(i => i.Tour.Id == parentId);
            foreach (var reservation in found)
            {
                _tourReservations.Remove(reservation);
            }
            _serializer.ToCsv(FilePath, _tourReservations);
        }

        public List<TourReservation> GetByGuest(User guest)
        {
            _tourReservations = _serializer.FromCsv(FilePath);
            return _tourReservations.FindAll(a => a.Guest.Id == guest.Id);
        }
    }
}
