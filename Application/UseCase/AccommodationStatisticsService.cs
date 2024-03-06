using System;
using SimsProject.Application.DTO;
using SimsProject.Application.Interface;
using System.Collections.Generic;
using System.Linq;
using SimsProject.Domain.Model;

namespace SimsProject.Application.UseCase
{
    public class AccommodationStatisticsService : IAccommodationStatisticsService
    {
        private readonly IAccommodationReservationService _reservationService;
        private readonly IAccommodationReservationRescheduleService _rescheduleService;
        private readonly IAccommodationReviewService _reviewService;

        public AccommodationStatisticsService()
        {
            _reservationService = Injector.CreateInstance<IAccommodationReservationService>();
            _rescheduleService = Injector.CreateInstance<IAccommodationReservationRescheduleService>();
            _reviewService = Injector.CreateInstance<IAccommodationReviewService>();
        }

        public int GetReservationCount(int accommodationId)
        {
            return _reservationService.GetByAccommodation(accommodationId)
                                                  .Where(r => !r.IsCanceled)
                                                  .ToList().Count;
        }

        public List<int> GetActiveYears(int accommodationId)
        {
            var reservations = _reservationService.GetByAccommodation(accommodationId);
            if (!reservations.Any()) return null;

            var earliestYear = reservations.Min(r => r.ArrivalDate.Year);
            var currentYear = DateTime.Now.Year;
            var years = Enumerable.Range(earliestYear, currentYear - earliestYear + 1).ToList();

            return years.OrderByDescending(n => n).ToList();
        }

        public List<AccommodationStatisticDto> GetStatistics(int accommodationId, int? year = null)
        {
            return year is null
                ? GetYearlyStatistics(accommodationId)
                : GetMonthlyStatistics(accommodationId, year.Value);
        }

        private List<AccommodationStatisticDto> GetMonthlyStatistics(int accommodationId, int year)
        {
            var monthlyStatistics = new List<AccommodationStatisticDto>();
            for (var i = 1; i <= 12; i++)
            {
                var statistics = CalculateStatistic(accommodationId, year, i);
                monthlyStatistics.Add(statistics);
            }

            return monthlyStatistics;
        }

        private List<AccommodationStatisticDto> GetYearlyStatistics(int accommodationId)
        {
            var yearlyStatistics = new List<AccommodationStatisticDto>();
            foreach (var year in GetActiveYears(accommodationId))
            {
                var statistics = CalculateStatistic(accommodationId, year);
                yearlyStatistics.Add(statistics);
            }
            
            return yearlyStatistics;

        }

        private AccommodationStatisticDto CalculateStatistic(int accommodationId, int year, int? month = null)
        {
            var reservations = month is not null
                ? _reservationService.GetAllByAccommodation(accommodationId).Where(r => r.ArrivalDate.Year == year && r.ArrivalDate.Month == month).ToList()
                : _reservationService.GetAllByAccommodation(accommodationId).Where(r => r.ArrivalDate.Year == year).ToList();

            var cancellationsCount = reservations.Count(r => r.IsCanceled);
            var reschedulesCount = CountReschedules(reservations);
            var renovationReferralsCount = CountRenovationReferrals(reservations);

            var statistics = new AccommodationStatisticDto(accommodationId, year, month, reservations.Count, cancellationsCount, reschedulesCount, renovationReferralsCount);

            return statistics;
        }

        private int CountReschedules(List<AccommodationReservation> reservations)
        {
            return reservations.SelectMany(r => _rescheduleService.GetByReservation(r)
                                .Where(rs => rs is not null && rs.RequestStatus == Status.Accepted))
                                .ToList().Count;
        }

        private int CountRenovationReferrals(List<AccommodationReservation> reservations)
        {
            return reservations.Select(r => _reviewService.GetByReservation(r))
                                .Where(r => r is not null && r.UrgencyLevel != 0)
                                .ToList().Count;
        }

        public int GetMostOccupied(int accommodationId, int? year)
        {
            return year is null
                ? GetMostOccupiedYear(accommodationId)
                : GetMostOccupiedMonth(accommodationId, year.Value);
        }

        private int GetMostOccupiedMonth(int accommodationId, int year)
        {
            var occupancyRates = new Dictionary<int, double>();
            for (var month = 1; month <= 12; month++)
            {
                var occupancyRate = CalculateOccupancyRate(accommodationId, year, month);
                occupancyRates[month] = occupancyRate;
            }

            return occupancyRates.MaxBy(kv => kv.Value).Key;
        }

        private int GetMostOccupiedYear(int accommodationId)
        {
            var occupancyRates = new Dictionary<int, double>();
            foreach (var year in GetActiveYears(accommodationId))
            {
                var occupancyRate = CalculateOccupancyRate(accommodationId, year);
                occupancyRates[year] = occupancyRate;
            }
            return occupancyRates.MaxBy(kv => kv.Value).Key;
        }

        public double CalculateOccupancyRate(int accommodationId, int year, int? month = null)
        {
            var reservations = _reservationService.GetByAccommodation(accommodationId);
            var daysInPeriod = month.HasValue ? DateTime.DaysInMonth(year, month.Value) : DateTime.IsLeapYear(year) ? 366 : 365;

            var occupiedDays = reservations
                .Where(reservation => IsOverlapping(reservation, year, month))
                .Sum(reservation => GetOccupiedDays(reservation, year, month));

            return (double)occupiedDays / daysInPeriod;
        }

        private static bool IsOverlapping(AccommodationReservation reservation, int year, int? month)
        {
            var isYearOverlapping = reservation.CheckoutDate.Year >= year && reservation.ArrivalDate.Year <= year;
            if (month is null) return isYearOverlapping;

            var isMonthOverlapping = reservation.CheckoutDate.Month >= month.Value && reservation.ArrivalDate.Month <= month.Value;
            return isYearOverlapping && isMonthOverlapping;
        }

        private static int GetOccupiedDays(AccommodationReservation reservation, int year, int? month)
        {
            var startDate = reservation.ArrivalDate.ToDateTime(TimeOnly.MinValue);
            var endDate = reservation.CheckoutDate.ToDateTime(TimeOnly.MinValue);

            DateTime firstDay, lastDay;
            if (month is null)
            {
                firstDay = new DateTime(year, 1, 1);
                lastDay = new DateTime(year, 12, DateTime.DaysInMonth(year, 12));
            }
            else
            {
                firstDay = new DateTime(year, month.Value, 1);
                lastDay = new DateTime(year, month.Value, DateTime.DaysInMonth(year, month.Value));
            }

            var start = startDate < firstDay ? firstDay : startDate;
            var end = endDate > lastDay ? lastDay : endDate;
            var occupiedDays = (end - start).Days + 1;

            return occupiedDays < 0 ? 0 : occupiedDays;
        }
    }
}