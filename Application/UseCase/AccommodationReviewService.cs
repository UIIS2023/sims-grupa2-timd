using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.Domain.RepositoryInterface;

namespace SimsProject.Application.UseCase
{
    public class AccommodationReviewService : IAccommodationReviewService
    {
        private readonly IAccommodationReviewRepository _reviewRepository;
        private readonly IAccommodationReviewImageService _imageService;
        public AccommodationReviewService()
        {
            _reviewRepository = Injector.CreateInstance<IAccommodationReviewRepository>();
            _imageService = Injector.CreateInstance<IAccommodationReviewImageService>();
        }

        public AccommodationReview GetById(int id)
        {
            return _reviewRepository.GetById(id);
        }

        public List<AccommodationReview> GetByOwner(User owner)
        {
            return _reviewRepository.GetByOwner(owner);
        }

        public AccommodationReview GetByReservation(AccommodationReservation reservation)
        {
            return _reviewRepository.GetByReservation(reservation);
        }

        public bool Exists(AccommodationReservation reservation)
        {
            return _reviewRepository.Exists(reservation);
        }

        public AccommodationReview CreateReview(AccommodationReview review, int id, ObservableCollection<Image> images)
        {
            _imageService.SaveByReview(id, images.ToList());
            return _reviewRepository.Save(review);
        }
    }
}
