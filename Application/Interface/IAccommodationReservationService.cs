using SimsProject.Application.Dto;
using SimsProject.Domain.Model;
using System;
using System.Collections.Generic;

namespace SimsProject.Application.Interface
{
    public interface IAccommodationReservationService : IService<AccommodationReservation>
    {
        List<AccommodationReservation> GetByOwner(User owner);
        List<AccommodationReservation> GetByGuest(User guest);
        List<AccommodationReservation> GetOngoingByOwner(User owner);
        List<AccommodationReservation> GetUpcomingByOwner(User owner);
        List<AccommodationReservation> GetPastByOwner(User owner);
        List<AccommodationReservation> GetUpcomingByGuest(User user);
        List<AccommodationReservation> GetCanceledByOwner(User owner);
        List<AccommodationReservation> GetPastByGuest(User user);
        List<AccommodationReservation> GetByAccommodation(int accommodationId);
        List<AccommodationReservation> GetAllByAccommodation(int accommodationId);
        List<AccommodationReservation> SearchAnywhereAnytime(int guestNumber, int stayLength, DateOnly date1, DateOnly date2);
        List<AccommodationReservation> GetByDateRange(User user, DateOnly startDate, DateOnly endDate);
        List<AccommodationReservation> GetByDateRangeCanceled(User user, DateOnly startDate, DateOnly endDate);
        List<DateRangeDto> FindAvailableDates(DateOnly startDate, DateOnly endDate, int length, Accommodation accommodation);
        List<DateRangeDto> SearchRecommendedDates(DateOnly date1, DateOnly date2, int stayLength, Accommodation ar);
        void CreateReservation(Accommodation acc, User currentUser, DateOnly date, int stayLength, int guestNumber);
        void CancelReservation(int canceledReservationId);
        AccommodationReservation Update(AccommodationReservation reservation);
        void Delete(AccommodationReservation reservation);
    }
}
