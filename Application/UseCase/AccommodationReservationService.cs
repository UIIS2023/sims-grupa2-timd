using System.Collections.Generic;
using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.Domain.RepositoryInterface;
using System;
using System.Linq;
using SimsProject.Application.Dto;

namespace SimsProject.Application.UseCase
{
    public class AccommodationReservationService : IAccommodationReservationService
    {
        private readonly IAccommodationReservationRepository _reservationRepository;
        private readonly IAccommodationRenovationService _renovationService;
        private readonly IAccommodationService _accommodationService;
        private readonly IAccommodationTypeService _accommodationTypeService;
        private readonly ILocationService _locationService;
        private readonly IAccommodationImageService _imageService;

        public AccommodationReservationService()
        {
            _reservationRepository = Injector.CreateInstance<IAccommodationReservationRepository>();
            _renovationService = Injector.CreateInstance<IAccommodationRenovationService>();
            _accommodationService = Injector.CreateInstance<IAccommodationService>();
            _accommodationTypeService = Injector.CreateInstance<IAccommodationTypeService>();
            _locationService = Injector.CreateInstance<ILocationService>();
            _imageService = Injector.CreateInstance<IAccommodationImageService>();
        }

        public AccommodationReservation GetById(int id)
        {
            return _reservationRepository.GetById(id);
        }

        public List<AccommodationReservation> GetByOwner(User owner)
        {
            return _reservationRepository.GetByOwner(owner);
        }
        public List<AccommodationReservation> GetByGuest(User guest)
        {
            return _reservationRepository.GetByGuest(guest);
        }

        public List<AccommodationReservation> GetOngoingByOwner(User owner)
        {
            return _reservationRepository.GetOngoingByOwner(owner);
        }

        public List<AccommodationReservation> GetUpcomingByOwner(User owner)
        {
            return _reservationRepository.GetUpcomingByOwner(owner);
        }

        public List<AccommodationReservation> GetPastByOwner(User owner)
        {
            return _reservationRepository.GetPastByOwner(owner);
        }
        public List<AccommodationReservation> GetCanceledByOwner(User owner)
        {
            return _reservationRepository.GetCanceledByOwner(owner);
        }

        public List<AccommodationReservation> GetUpcomingByGuest(User user)
        {
            return _reservationRepository.GetUpcomingByGuest(user);
        }

        public List<AccommodationReservation> GetPastByGuest(User user)
        {
            return _reservationRepository.GetPastByGuest(user);
        }

        public List<AccommodationReservation> GetByAccommodation(int accommodationId)
        {
            return _reservationRepository.GetByAccommodation(accommodationId);
        }

        public List<AccommodationReservation> GetAllByAccommodation(int accommodationId)
        {
            return _reservationRepository.GetAllByAccommodation(accommodationId);
        }

        public List<AccommodationReservation> GetByDateRange(User user, DateOnly startDate, DateOnly endDate)
        {
            return _reservationRepository.GetAll()
                .Where(r => r.ArrivalDate <= endDate && r.CheckoutDate >= startDate && r.Guest.Id == user.Id && !r.IsCanceled)
                .OrderBy(r => r.ArrivalDate)
                .ToList();
        }

        public List<AccommodationReservation> GetByDateRangeCanceled(User user, DateOnly startDate, DateOnly endDate)
        {
            return _reservationRepository.GetAll()
                .Where(r => r.ArrivalDate <= endDate && r.CheckoutDate >= startDate && r.Guest.Id == user.Id && r.IsCanceled)
                .OrderBy(r => r.ArrivalDate)
                .ToList();
        }

        private Accommodation Populate(Accommodation accommodation)
        {
            accommodation.Type = _accommodationTypeService.GetById(accommodation.Type.Id);
            accommodation.Cover = _imageService.GetById(accommodation.Cover.Id);
            accommodation.Location = _locationService.GetById(accommodation.Location.Id);
            accommodation.Images = _imageService.GetByAccommodation(_accommodationService.GetById(accommodation.Id));
            return accommodation;
        }

        public List<AccommodationReservation> SearchAnywhereAnytime(int guestNumber, int stayLength, DateOnly date1, DateOnly date2)
        {
            var d1 = date1 == DateOnly.MinValue ? DateOnly.FromDateTime(DateTime.Today) : date1;
            var d2 = date2 == DateOnly.MinValue ? DateOnly.FromDateTime(DateTime.Today.AddMonths(3)) : date2;
            var accommodations= new List<Accommodation>(_accommodationService.Search(null, guestNumber.ToString(), stayLength.ToString(), null, null, null));
            return (from accommodation in accommodations select Populate(accommodation) into temp let valid = new List<DateRangeDto>(FindAvailableDates(d1, d2, stayLength, temp)) from range in valid select new AccommodationReservation(temp, new User("", "", UserType.Guest1), temp.Owner, range.StartDate, stayLength, guestNumber)).ToList();
        }

        public List<DateRangeDto> FindAvailableDates(DateOnly startDate, DateOnly endDate, int length, Accommodation accommodation)
        {
            List<DateRangeDto> availableDates = new();
            for (var date = startDate; date <= endDate.AddDays(-length + 1); date = date.AddDays(1))
            {
                if (IsDateAvailable(accommodation.Id, date, length))
                {
                    var availableDate = new DateRangeDto(date, date.AddDays(length - 1));
                    availableDates.Add(availableDate);
                }
            }
            return availableDates;
        }

        public List<DateRangeDto> SearchRecommendedDates(DateOnly date1, DateOnly date2, int stayLength, Accommodation ar)
        {
            List<DateRangeDto> valid = new();
            var recommendedDay = 0;
            while (recommendedDay < 10)
            {
                if (IsDateAvailable(ar.Id, date2, stayLength))
                {
                    recommendedDay++;
                    DateRangeDto validDate = new(date2, date2.AddDays(stayLength - 1));
                    valid.Add(validDate);
                }
                date2 = date2.AddDays(1);
            }
            return valid;
        }

        private bool IsDateAvailable(int accommodationId, DateOnly startDate, int length)
        {
            var isReserved = IsDateReserved(accommodationId, startDate, length);
            if (isReserved) return false;

            var isUnderRenovation = IsDateUnderRenovation(accommodationId, startDate, length);
            return !isUnderRenovation;
        }

        private bool IsDateReserved(int accommodationId, DateOnly startDate, int length)
        {
            return (from ar in _reservationRepository.GetByAccommodation(accommodationId)
                    let arrivalDay = ar.ArrivalDate.AddDays(-length + 1)
                    let departureDay = ar.ArrivalDate.AddDays(ar.StayLength - 1)
                    where startDate >= arrivalDay && startDate <= departureDay
                    select arrivalDay).Any();
        }

        private bool IsDateUnderRenovation(int accommodationId, DateOnly startDate, int length)
        {
            return (from r in _renovationService.GetByAccommodation(accommodationId)
                    let endDate = startDate.AddDays(length - 1)
                    where startDate >= r.StartDate && endDate <= r.EndDate
                    select r).Any();
        }

        public void CreateReservation(Accommodation acc, User currentUser, DateOnly date, int stayLength, int guestNumber)
        {
            AccommodationReservation newReservation = new(acc, currentUser, acc.Owner, date, stayLength, guestNumber);
            _reservationRepository.Save(newReservation);
        }

        public void CancelReservation(int canceledReservationId)
        {
            var canceledReservation = _reservationRepository.GetById(canceledReservationId);
            canceledReservation.IsCanceled = true;
            _reservationRepository.Update(canceledReservation);
        }

        public AccommodationReservation Update(AccommodationReservation accommodation)
        {
            return _reservationRepository.Update(accommodation);
        }

        public void Delete(AccommodationReservation accommodation)
        { 
            _reservationRepository.Delete(accommodation);
        }
    }
}
