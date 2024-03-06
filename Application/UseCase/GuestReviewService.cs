using System.Collections.Generic;
using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using User = SimsProject.Domain.Model.User;
using SimsProject.Domain.RepositoryInterface;

namespace SimsProject.Application.UseCase
{
    public class GuestReviewService : IGuestReviewService
    {
        private readonly IGuestReviewRepository _reviewRepository;

        public GuestReviewService()
        {
            _reviewRepository = Injector.CreateInstance<IGuestReviewRepository>();
        }

        public GuestReview GetById(int id)
        {
            return _reviewRepository.GetById(id);
        }

        public bool Exists(AccommodationReservation reservation)
        {
            return _reviewRepository.Exists(reservation);
        }

        public GuestReview CreateReview(GuestReview guestReview)
        {
            var savedGuestReview = _reviewRepository.Save(guestReview);
            return savedGuestReview;
        }

        public List<GuestReview> GetByGuest(User guest)
        {
            return _reviewRepository.GetByGuest(guest);
        }
    }
}
