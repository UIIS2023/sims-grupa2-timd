using SimsProject.Domain.Model;
using SimsProject.Domain.RepositoryInterface;
using SimsProject.Serializer;
using System.Collections.Generic;
using System.Linq;

namespace SimsProject.Repository
{
    public class TourReviewCsvRepository : ITourReviewRepository
    {
        private const string FilePath = "../../../Resources/Data/tourReviews.csv";

        private readonly Serializer<TourReview> _serializer;

        private List<TourReview> _tourReviews;

        public TourReviewCsvRepository()
        {
            _serializer = new Serializer<TourReview>();
            _tourReviews = _serializer.FromCsv(FilePath);
        }

        public List<TourReview> GetAll()
        {
            List<TourReview> tourReviews = _serializer.FromCsv(FilePath);
            return tourReviews;
        }

        public TourReview GetById(int id)
        {
            _tourReviews = _serializer.FromCsv(FilePath);
            return _tourReviews.FirstOrDefault(l => l.Id == id);
        }

        public TourReview Save(TourReview tourReviews)
        {
            tourReviews.Id = NextId();
            _tourReviews = _serializer.FromCsv(FilePath);
            _tourReviews.Add(tourReviews);
            _serializer.ToCsv(FilePath, _tourReviews);
            return tourReviews;
        }

        TourReview IRepository<TourReview>.Update(TourReview review)
        {
            _tourReviews = GetAll();
            var currentReview = _tourReviews.Find(d => d.Id == review.Id);

            currentReview.IsValid = review.IsValid;
            currentReview.CheckPoint = review.CheckPoint;

            var index = _tourReviews.IndexOf(currentReview);

            _tourReviews.Remove(currentReview);
            _tourReviews.Insert(index, currentReview);
            _serializer.ToCsv(FilePath, _tourReviews);

            return review;
        }

        public int NextId()
        {
            _tourReviews = _serializer.FromCsv(FilePath);
            if (_tourReviews.Count < 1)
            {
                return 1;
            }
            return _tourReviews.Max(r => r.Id) + 1;
        }

        public void Delete(TourReview tourReview)
        {
            _tourReviews = _serializer.FromCsv(FilePath);
            TourReview found = _tourReviews.Find(r => r.Id == tourReview.Id);
            _tourReviews.Remove(found);
            _serializer.ToCsv(FilePath, _tourReviews);
        }

        public TourReview UpdateById(int tourReviewId)
        {
            _tourReviews = GetAll();
            var currentReview = _tourReviews.Find(d => d.Id == tourReviewId);
            currentReview.IsValid = false;
            var index = _tourReviews.IndexOf(currentReview);

            _tourReviews.Remove(currentReview);
            _tourReviews.Insert(index, currentReview);
            _serializer.ToCsv(FilePath, _tourReviews);
            return currentReview;
        }

        public void DeleteAllByParentId(int parentId)
        {
            _tourReviews = _serializer.FromCsv(FilePath);
            var found = _tourReviews.FindAll(i => i.Tour.Id == parentId);
            foreach (var tourReview in found)
            {
                _tourReviews.Remove(tourReview);
            }
            _serializer.ToCsv(FilePath, _tourReviews);
        }

        /*public List<TourReview> GetByGuide(User guide)
        {
            _tourReviews = _serializer.FromCsv(FilePath);
            return _tourReviews.FindAll(r => r.Guide.Id == guide.Id);
        }*/

        public List<TourReview> GetByGuest(User guest)
        {
            _tourReviews = _serializer.FromCsv(FilePath);
            return _tourReviews.FindAll(r => r.Guest.Id == guest.Id);
        }

        public List<TourReview> GetByTour(Tour tour)
        {
            _tourReviews = _serializer.FromCsv(FilePath);
            return _tourReviews.FindAll(r => r.Tour.Id == tour.Id);
        }

        public TourReview GetByReservation(TourReservation reservation)
        {
            _tourReviews = _serializer.FromCsv(FilePath);
            return _tourReviews.FirstOrDefault(r => r.Reservation.Id == reservation.Id);
        }

        public void UpdateReview(TourReview review)
        {
            _tourReviews = GetAll();
            var currentReview = _tourReviews.Find(d => d.Id == review.Id);

            currentReview.IsValid = review.IsValid;
            currentReview.CheckPoint = review.CheckPoint;

            var index = _tourReviews.IndexOf(currentReview);

            _tourReviews.Remove(currentReview);
            _tourReviews.Insert(index, currentReview);
            _serializer.ToCsv(FilePath, _tourReviews);
        }

        public TourReview Update(TourReview review)
        {
            _tourReviews = GetAll();
            var currentReview = _tourReviews.Find(d => d.Id == review.Id);

            currentReview.IsValid = review.IsValid;
            currentReview.CheckPoint = review.CheckPoint;

            var index = _tourReviews.IndexOf(currentReview);

            _tourReviews.Remove(currentReview);
            _tourReviews.Insert(index, currentReview);
            _serializer.ToCsv(FilePath, _tourReviews);
            return currentReview;
        }
    }
}
