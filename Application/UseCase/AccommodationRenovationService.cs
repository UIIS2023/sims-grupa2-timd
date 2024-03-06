using System;
using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.Domain.RepositoryInterface;
using System.Collections.Generic;
using System.Linq;

namespace SimsProject.Application.UseCase
{
    public class AccommodationRenovationService : IAccommodationRenovationService
    {
        private readonly IAccommodationRenovationRepository _renovationRepository;

        public AccommodationRenovationService()
        {
            _renovationRepository = Injector.CreateInstance<IAccommodationRenovationRepository>();
        }

        public AccommodationRenovation GetById(int id)
        {
            return _renovationRepository.GetById(id);
        }

        public List<AccommodationRenovation> GetByAccommodation(int accommodationId)
        {
            return _renovationRepository.GetByAccommodation(accommodationId);
        }

        public AccommodationRenovation GetMostRecentByAccommodation(Accommodation accommodation)
        {
            var renovations = _renovationRepository.GetByAccommodation(accommodation.Id)
                                                   .Where(r => r.EndDate <= DateOnly.FromDateTime(DateTime.Today))
                                                   .OrderByDescending(r => r.EndDate);

            var latestRenovation = renovations.FirstOrDefault();

            return latestRenovation;
        }

        public List<AccommodationRenovation> GetByDateRange(DateOnly startDate, DateOnly endDate)
        {
            return _renovationRepository.GetAll()
                .Where(r => r.StartDate <= endDate && r.EndDate >= startDate)
                .OrderBy(r => r.StartDate)
                .ToList();
        }

        public List<AccommodationRenovation> GetUpcomingByOwner(User owner)
        {
            var allRenovations = new List<AccommodationRenovation>();
            allRenovations.AddRange(_renovationRepository.GetUpcomingByOwner(owner));
            allRenovations.AddRange(_renovationRepository.GetOngoingByOwner(owner));
            return allRenovations;
        }

        public List<AccommodationRenovation> GetPastByOwner(User owner)
        {
            return _renovationRepository.GetPastByOwner(owner);
        }

        public AccommodationRenovation ScheduleRenovation(AccommodationRenovation renovation)
        {
            var savedRenovation = _renovationRepository.Save(renovation);
            return savedRenovation;
        }

        public void CancelRenovation(int id)
        {
            var accommodation = _renovationRepository.GetById(id);
            _renovationRepository.Delete(accommodation);
        }
    }
}