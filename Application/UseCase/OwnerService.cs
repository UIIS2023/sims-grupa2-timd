using System;
using System.Linq;
using SimsProject.Application.Dto;
using SimsProject.Application.Interface;
using SimsProject.Domain.Model;

namespace SimsProject.Application.UseCase
{
    public class OwnerService : IOwnerService
    {
        private readonly IAccommodationReviewService _reviewService;
        private readonly IAccommodationService _accommodationService;
        private readonly IAccommodationReservationService _reservationService;
        private readonly IAccommodationStatisticsService _statisticsService;

        public OwnerService()
        {
            _reviewService = Injector.CreateInstance<IAccommodationReviewService>();
            _accommodationService = Injector.CreateInstance<IAccommodationService>();
            _reservationService = Injector.CreateInstance<IAccommodationReservationService>();
            _statisticsService = Injector.CreateInstance<IAccommodationStatisticsService>();
        }

        public double GetAverageRating(User owner)
        {
            var reviews = _reviewService.GetByOwner(owner);
            if (reviews == null || !reviews.Any()) return 0;

            return reviews.Average(r => r.OwnerGrade + r.CleanlinessGrade);
        }

        public int GetReviewCount(User owner)
        {
            var reviews = _reviewService.GetByOwner(owner);
            if (reviews == null || !reviews.Any()) return 0;

            return reviews.Count;
        }

        public int GetAccommodationCount(User owner)
        {
            return _accommodationService.GetByOwner(owner).Count;
        }

        public int GetPastReservationCount(User owner)
        {
            return _reservationService.GetPastByOwner(owner).Count;
        }

        public bool GetSuperOwnerStatus(User owner)
        {
            return AccommodationOwner.IsSuperOwner(GetReviewCount(owner), GetAverageRating(owner));
        }

        public AccommodationRecommendationDto GetRecommendations(User owner)
        {
            var accommodations = _accommodationService.GetByOwner(owner);

            if (accommodations.Count <= 1) return null;

            var bestToWorstAccommodation = accommodations.OrderByDescending(a => _statisticsService.GetReservationCount(a.Id))
                                            .ThenByDescending(a => _statisticsService.CalculateOccupancyRate(a.Id, DateTime.Now.Year))
                                            .ToList();

            var openRecommendation = bestToWorstAccommodation.First();
            var closeRecommendation = bestToWorstAccommodation.Last();

            return new AccommodationRecommendationDto(openRecommendation.Location, closeRecommendation.Location);
        }
    }
}
