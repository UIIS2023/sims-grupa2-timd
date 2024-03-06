using SimsProject.Domain.Model;
using SimsProject.Serializer;
using System.Collections.Generic;
using System.Linq;
using SimsProject.Domain.RepositoryInterface;

namespace SimsProject.Repository
{
    public class AccommodationReviewCsvRepository : IAccommodationReviewRepository
    {
        private const string FilePath = "../../../Resources/Data/accommodationReviews.csv";

        private readonly Serializer<AccommodationReview> _serializer;

        private List<AccommodationReview> _accommodationReviews;

        public AccommodationReviewCsvRepository()
        {
            _serializer = new Serializer<AccommodationReview>();
            _accommodationReviews = _serializer.FromCsv(FilePath);
        }

        public List<AccommodationReview> GetAll()
        {
            List<AccommodationReview> accommodationReviews = _serializer.FromCsv(FilePath);
            return accommodationReviews;
        }

        public AccommodationReview GetById(int id)
        {
            _accommodationReviews = _serializer.FromCsv(FilePath);
            return _accommodationReviews.FirstOrDefault(r => r.Id == id);
        }

        public int NextId()
        {
            _accommodationReviews = _serializer.FromCsv(FilePath);
            if (_accommodationReviews.Count < 1)
            {
                return 1;
            }
            return _accommodationReviews.Max(r => r.Id) + 1;
        }

        public AccommodationReview Save(AccommodationReview accommodationReviews)
        {
            accommodationReviews.Id = NextId();
            _accommodationReviews = _serializer.FromCsv(FilePath);
            _accommodationReviews.Add(accommodationReviews);
            _serializer.ToCsv(FilePath, _accommodationReviews);
            return accommodationReviews;
        }

        public AccommodationReview Update(AccommodationReview accommodationReview)
        {
            _accommodationReviews = _serializer.FromCsv(FilePath);
            AccommodationReview current = _accommodationReviews.Find(r => r.Id == accommodationReview.Id);
            int index = _accommodationReviews.IndexOf(current);
            _accommodationReviews.Remove(current);
            _accommodationReviews.Insert(index, accommodationReview);
            _serializer.ToCsv(FilePath, _accommodationReviews);
            return accommodationReview;
        }

        public void Delete(AccommodationReview accommodationReview)
        {
            _accommodationReviews = _serializer.FromCsv(FilePath);
            AccommodationReview found = _accommodationReviews.Find(r => r.Id == accommodationReview.Id);
            _accommodationReviews.Remove(found);
            _serializer.ToCsv(FilePath, _accommodationReviews);
        }

        public List<AccommodationReview> GetByOwner(User owner)
        {
            _accommodationReviews = _serializer.FromCsv(FilePath);
            return _accommodationReviews.FindAll(r => r.Owner.Id == owner.Id);
        }

        public List<AccommodationReview> GetByGuest(User guest)
        {
            _accommodationReviews = _serializer.FromCsv(FilePath);
            return _accommodationReviews.FindAll(r => r.Guest.Id == guest.Id);
        }

        public AccommodationReview GetByReservation(AccommodationReservation reservation)
        {
            _accommodationReviews = _serializer.FromCsv(FilePath);
            return _accommodationReviews.FirstOrDefault(r => r.Reservation.Id == reservation.Id);
        }

        public bool Exists(AccommodationReservation reservation)
        {
            _accommodationReviews = _serializer.FromCsv(FilePath);
            return _accommodationReviews.Exists(r => r.Reservation.Id == reservation.Id);
        }
    }
}
