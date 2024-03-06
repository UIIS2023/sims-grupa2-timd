using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.Domain.RepositoryInterface;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SimsProject.Application.UseCase
{
    public class TourReviewService : ITourReviewService
    {

        private readonly ITourReviewRepository _tourReviewRepository;
        private readonly ITourService _tourService;
        private readonly ITourDateService _tourDateService;
        private readonly ITourReviewImageService _tourReviewImageService;
        private readonly ITourAttendanceService _tourAttendanceService;

        public TourReviewService()
        {
            _tourReviewRepository = Injector.CreateInstance<ITourReviewRepository>();
            _tourService = Injector.CreateInstance<ITourService>();
            _tourDateService = Injector.CreateInstance<ITourDateService>();
            _tourReviewImageService = Injector.CreateInstance<ITourReviewImageService>();
            _tourAttendanceService = Injector.CreateInstance<ITourAttendanceService>();
        }

        public TourReview GetById(int id)
        {
            return _tourReviewRepository.GetById(id);
        }

        public List<TourReview> GetAll()
        {
            return _tourReviewRepository.GetAll();
        }

        public void DeleteAllByParentId(int parentId)
        {
            _tourReviewRepository.DeleteAllByParentId(parentId);
        }

        public TourReview Update(TourReview tourReview)
        {
            return _tourReviewRepository.Update(tourReview);
        }

        public TourReview UpdateById(int tourReviewId)
        {
            return _tourReviewRepository.UpdateById(tourReviewId);
        }

        public List<TourReview> GetByGuest(User guest)
        {
            return _tourReviewRepository.GetByGuest(guest);
        }

        public List<TourReview> GetFilteredReviews(int tourId)
        {
            return _tourReviewRepository.GetAll().Where(review =>
                _tourService.GetAll().Any(tour => tour.Id == review.Tour.Id && tour.Id == tourId) &&
                _tourDateService.GetAll().Any(date => date.Tour.Id == review.Tour.Id &&
                                                      date.Id == review.TourDate.Id)).ToList();
        }

        public void CreateTourReview(User loggedInUser, int knowledge, int language, int interestigness, string comment, List<TourAttendance> attendances, TourReservation reservation,ObservableCollection<Image> images, TourDate date) 
        {
            var pointId = _tourAttendanceService.GetCheckPoint(loggedInUser, reservation.Tour).Id;
            foreach (var attendance in attendances) 
            {
                if (CanUserMakeReview(reservation,attendance)) 
                {
                    var review = new TourReview(knowledge, language, interestigness, comment, reservation.Guest, reservation, images.ToList(), date, reservation.Tour);
                    review.CheckPoint.Id = pointId;
                    _tourReviewRepository.Save(review);
                    SaveReviewImages(images.ToList(), review);
                }
            }
        }

        public void SaveReviewImages(List<Image> images, TourReview review) 
        {
            _tourReviewImageService.SaveAllByParentId(review.Id, images);
        }

        public bool CanUserMakeReview(TourReservation reservation,TourAttendance attendance) 
        {
            var isValidTour = reservation.Tour.Id == attendance.Tour.Id;
            var isGuestValid = reservation.Guest.Id == attendance.User.Id;
            var notAlreadyReviewed = !ExistsReview(reservation);

            return isValidTour && isGuestValid && notAlreadyReviewed;
        }

        public bool ExistsReview(TourReservation reservation) 
        {
            var reviews = _tourReviewRepository.GetAll();
            return reviews.Any(review => review.Reservation.Id == reservation.Id);
        }

        public List<TourReview> GetByTour(Tour tour)
        {
            return _tourReviewRepository.GetByTour(tour);
        }
    }
}
