using System;
using System.Collections.Generic;
using SimsProject.Domain.Model;

namespace SimsProject.Application.Interface
{
    public interface IAccommodationRenovationService : IService<AccommodationRenovation>
    {
        List<AccommodationRenovation> GetByAccommodation(int accommodationId);
        AccommodationRenovation GetMostRecentByAccommodation(Accommodation accommodation);
        List<AccommodationRenovation> GetByDateRange(DateOnly startDate, DateOnly endDate);
        List<AccommodationRenovation> GetUpcomingByOwner(User owner);
        List<AccommodationRenovation> GetPastByOwner(User owner);
        AccommodationRenovation ScheduleRenovation(AccommodationRenovation renovation);
        void CancelRenovation(int id);
    }
}