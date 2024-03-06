using SimsProject.Domain.Model;
using SimsProject.Serializer;
using System.Collections.Generic;
using System.Linq;
using SimsProject.Domain.RepositoryInterface;
using System;

namespace SimsProject.Repository
{
    public class AccommodationReservationCsvRepository : IAccommodationReservationRepository
    {
        private const string FilePath = "../../../Resources/Data/accommodationReservations.csv";

        private readonly Serializer<AccommodationReservation> _serializer;

        private List<AccommodationReservation> _accommodationReservations;

        public AccommodationReservationCsvRepository()
        {
            _serializer = new Serializer<AccommodationReservation>();
            _accommodationReservations = _serializer.FromCsv(FilePath);
        }

        public List<AccommodationReservation> GetAll()
        {
            return _serializer.FromCsv(FilePath);
        }

        public AccommodationReservation GetById(int id)
        {
            _accommodationReservations = _serializer.FromCsv(FilePath);
            return _accommodationReservations.FirstOrDefault(a => a.Id == id);
        }

        public int NextId()
        {
            _accommodationReservations = _serializer.FromCsv(FilePath);
            if (_accommodationReservations.Count < 1)
            {
                return 1;
            }
            return _accommodationReservations.Max(c => c.Id) + 1;
        }

        public AccommodationReservation Save(AccommodationReservation accommodationReservation) 
        {
            accommodationReservation.Id = NextId();
            _accommodationReservations = _serializer.FromCsv(FilePath);
            _accommodationReservations.Add(accommodationReservation);
            _serializer.ToCsv(FilePath, _accommodationReservations);
            return accommodationReservation;
        }

        public AccommodationReservation Update(AccommodationReservation accommodationReservation)
        {
            _accommodationReservations = _serializer.FromCsv(FilePath);
            AccommodationReservation current = _accommodationReservations.Find(a => a.Id == accommodationReservation.Id);
            int index = _accommodationReservations.IndexOf(current);
            _accommodationReservations.Remove(current);
            _accommodationReservations.Insert(index, accommodationReservation);
            _serializer.ToCsv(FilePath, _accommodationReservations);
            return accommodationReservation;
        }

        public void Delete(AccommodationReservation accommodationReservation)
        {
            _accommodationReservations = _serializer.FromCsv(FilePath);
            AccommodationReservation founded = _accommodationReservations.Find(c => c.Id == accommodationReservation.Id);
            _accommodationReservations.Remove(founded);
            _serializer.ToCsv(FilePath, _accommodationReservations);
        }

        public List<AccommodationReservation> GetByAccommodation(int accommodationId)
        {
            _accommodationReservations = _serializer.FromCsv(FilePath);
            return _accommodationReservations.FindAll(r => r.Accommodation.Id == accommodationId && !r.IsCanceled);
        }

        public List<AccommodationReservation> GetAllByAccommodation(int accommodationId)
        {
            _accommodationReservations = _serializer.FromCsv(FilePath);
            return _accommodationReservations.FindAll(a => a.Accommodation.Id == accommodationId);
        }

        public List<AccommodationReservation> GetByOwner(User owner)
        {
            _accommodationReservations = _serializer.FromCsv(FilePath);
            return _accommodationReservations.FindAll(a => a.Owner.Id == owner.Id && !a.IsCanceled);
        }

        public List<AccommodationReservation> GetCanceledByOwner(User owner)
        {
            _accommodationReservations = _serializer.FromCsv(FilePath);
            return _accommodationReservations.FindAll(a => a.Owner.Id == owner.Id && a.IsCanceled);
        }

        public List<AccommodationReservation> GetOngoingByOwner(User owner)
        {
            return (from reservation in _accommodationReservations.FindAll(a => a.Owner.Id == owner.Id && !a.IsCanceled)
                    let hasStarted = reservation.ArrivalDate.CompareTo(DateOnly.FromDateTime(DateTime.Today)) <= 0
                    let hasPassed = reservation.CheckoutDate.CompareTo(DateOnly.FromDateTime(DateTime.Today)) <= 0
                    where hasStarted && !hasPassed
                    select reservation).ToList();
        }

        public List<AccommodationReservation> GetUpcomingByOwner(User owner)
        {
            return (from reservation in _accommodationReservations.FindAll(a => a.Owner.Id == owner.Id && !a.IsCanceled)
                    let hasStarted = reservation.ArrivalDate.CompareTo(DateOnly.FromDateTime(DateTime.Today)) <= 0
                    where !hasStarted
                    select reservation).ToList();
        }

        public List<AccommodationReservation> GetPastByOwner(User owner)
        {
            return (from reservation in _accommodationReservations.FindAll(a => a.Owner.Id == owner.Id && !a.IsCanceled)
                    let hasPassed = reservation.CheckoutDate.CompareTo(DateOnly.FromDateTime(DateTime.Today)) <= 0
                    where hasPassed
                    select reservation).ToList();
        }

        public List<AccommodationReservation> GetUpcomingByGuest(User user)
        {
            return _accommodationReservations.Where(reservation => reservation.CheckoutDate >= DateOnly.FromDateTime(DateTime.Today) && !reservation.IsCanceled && reservation.Guest.Id == user.Id).ToList();
        }

        public List<AccommodationReservation> GetPastByGuest(User user)
        {
            return _accommodationReservations.Where(reservation => reservation.CheckoutDate < DateOnly.FromDateTime(DateTime.Today) && 
                                                                   reservation.Guest.Id == user.Id && !reservation.IsCanceled &&
                                                                   reservation.CheckoutDate.AddDays(5) >=
                                                                   DateOnly.FromDateTime(DateTime.Today)).ToList();
        }
        public List<AccommodationReservation> GetByGuest(User guest)
        {
            _accommodationReservations = _serializer.FromCsv(FilePath);
            return _accommodationReservations.FindAll(a => a.Guest.Id == guest.Id && !a.IsCanceled);
        }
    }
}
