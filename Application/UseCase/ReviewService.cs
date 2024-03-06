using System;
using System.Collections.Generic;
using System.Linq;
using SimsProject.Application.Interface;
using SimsProject.Domain.Model;

namespace SimsProject.Application.UseCase
{
    public class ReviewService : IReviewService
    {
        private readonly IAccommodationReviewService _accommodationReviewService;
        private readonly IGuestReviewService _guestReviewService;
        private readonly IAccommodationReservationService _reservationService;

        private const int ReviewPeriodDays = 5;

        public ReviewService()
        {
            _accommodationReviewService = Injector.CreateInstance<IAccommodationReviewService>();
            _guestReviewService = Injector.CreateInstance<IGuestReviewService>();
            _reservationService = Injector.CreateInstance<IAccommodationReservationService>();
        }

        public List<GuestReview> GetReviewsByGuest(User guest)
        {
            List<GuestReview> reviews = new(_guestReviewService.GetByGuest(guest)
                .Where(review => _accommodationReviewService.Exists(review.Reservation))
                .ToList());
            return reviews;
        }

        public List<AccommodationReview> GetGuestReviewedByOwner(User owner)
        {
            return _accommodationReviewService.GetByOwner(owner)
                .Where(review => _guestReviewService.Exists(review.Reservation))
                .ToList();
        }

        public List<AccommodationReservation> GetUnreviewedByOwner(User owner)
        {
            return (from reservation in _reservationService.GetPastByOwner(owner)
                    let isReviewed = _guestReviewService.Exists(reservation)
                    let isReviewPeriodExpired = reservation.CheckoutDate <
                                            DateOnly.FromDateTime(DateTime.Today.AddDays(-ReviewPeriodDays))
                    where !isReviewed && !isReviewPeriodExpired
                    select reservation).ToList();
        }

        public List<AccommodationReservation> GetUnreviewedByGuest(User guest)
        {
            return (from reservation in _reservationService.GetPastByGuest(guest)
                    let isReviewed = _accommodationReviewService.Exists(reservation)
                    let isReviewPeriodExpired = reservation.CheckoutDate <
                                            DateOnly.FromDateTime(DateTime.Today.AddDays(-ReviewPeriodDays))
                    where !isReviewed && !isReviewPeriodExpired
                    select reservation).ToList();
        }
    }
}
