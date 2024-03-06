using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using System;
using System.Linq;

namespace SimsProject.Application.UseCase
{
    public class GuideService : IGuideService
    {
        #region SERVICES

        private readonly ITourReservationService _tourReservationService;
        private readonly ITourReviewImageService _tourReviewImageService;
        private readonly ITourAttendanceService _tourAttendanceService;
        private readonly ITourVoucherService _tourVoucherService;
        private readonly ITourReviewService _tourReviewService;
        private readonly ICheckPointService _checkPointService;
        private readonly ITourImageService _tourImageService;
        private readonly ITourDateService _tourDateService;
        private readonly ITourService _tourService;

        #endregion

        public GuideService()
        {
            _tourService = Injector.CreateInstance<ITourService>();
            _tourReviewService = Injector.CreateInstance<ITourReviewService>();
            _tourDateService = Injector.CreateInstance<ITourDateService>();
            _tourImageService = Injector.CreateInstance<ITourImageService>();
            _checkPointService = Injector.CreateInstance<ICheckPointService>();
            _tourReservationService = Injector.CreateInstance<ITourReservationService>();
            _tourAttendanceService = Injector.CreateInstance<ITourAttendanceService>();
            _tourReviewImageService = Injector.CreateInstance<ITourReviewImageService>();
            _tourVoucherService = Injector.CreateInstance<ITourVoucherService>();
        }

        public (int Count, string Language) GetMostFrequentLanguageCount(User guide)
        {
            var tours = _tourService.GetByGuide(guide);
            var languageCounts = tours.GroupBy(t => t.Language)
                .Select(g => new { Language = g.Key, Count = g.Count() })
                .MaxBy(x => x.Count);

            return (languageCounts?.Count ?? 0, languageCounts?.Language);
        }

        public double GetAverageRatingForLanguage(User guide)
        {
            var (_, language) = GetMostFrequentLanguageCount(guide);

            var tours = _tourService.GetByGuide(guide);
            var totalRating = 0;
            var reviewCount = 0;

            foreach (var tourReviews in from tour in tours where tour.Language == language select _tourReviewService.GetByTour(tour))
            {
                totalRating += tourReviews.Sum(tr => tr.GuideLanguage);
                reviewCount += tourReviews.Count;
            }

            if (reviewCount <= 0) return 0.0;
            var averageRating = (double)totalRating / reviewCount;

            return averageRating;
        }

        public bool GetSuperGuideStatus(User guide)
        {
            return TourGuide.IsSuperGuide(GetMostFrequentLanguageCount(guide).Count, GetAverageRatingForLanguage(guide));
        }

        public void Resign(User guide)
        {
            foreach (var tour in _tourService.GetByGuide(guide))
            {
                SendVouchers(tour);
                var deletedTourReviews = _tourReviewService.GetByTour(tour);

                _tourDateService.DeleteAllByParentId(tour.Id);
                _tourImageService.DeleteAllByParentId(tour.Id);
                _checkPointService.DeleteByParentId(tour.Id);
                _tourReservationService.DeleteAllByParentId(tour.Id);
                _tourReviewService.DeleteAllByParentId(tour.Id);
                _tourAttendanceService.DeleteAllByParentId(tour.Id);

                foreach (var tourReview in deletedTourReviews)
                {
                    _tourReviewImageService.DeleteAllByParentId(tourReview.Id);
                }

                _tourService.Delete(tour);
            }
        }

        private void SendVouchers(Tour tour)
        {
            var guests = _tourReservationService.GetByTour(tour).Select(reservation => reservation.Guest).ToList();

            var todayDate = DateTime.Today;
            var voucherDate = todayDate.AddYears(2);

            foreach (var tourVoucher in guests.Select(guest => new TourVoucher(guest, voucherDate)))
            {
                _tourVoucherService.Save(tourVoucher);
            }
        }
    }
}