using System.Collections.Generic;
using SimsProject.Application.DTO;

namespace SimsProject.Application.Interface
{
    public interface IAccommodationStatisticsService
    {
        int GetReservationCount(int accommodationId);
        List<int> GetActiveYears(int accommodationId);
        List<AccommodationStatisticDto> GetStatistics(int accommodationId, int? year);
        int GetMostOccupied(int accommodationId, int? year);
        double CalculateOccupancyRate(int accommodationId, int year, int? month = null);
    }
}