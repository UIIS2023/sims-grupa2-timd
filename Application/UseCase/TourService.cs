using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.Domain.RepositoryInterface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimsProject.Application.UseCase
{
    public class TourService : ITourService
    {
        private readonly ITourRepository _tourRepository;
        private readonly ITourDateService _tourDateService;
        private readonly ITourImageService _tourImageService;

        public TourService()
        {
            _tourRepository = Injector.CreateInstance<ITourRepository>();
            _tourDateService = Injector.CreateInstance<ITourDateService>();
            _tourImageService = Injector.CreateInstance<ITourImageService>();
        }

        public Tour GetById(int id)
        {
            return _tourRepository.GetById(id);
        }

        public List<Tour> GetByGuide(User guide)
        {
            return _tourRepository.GetByGuide(guide);
        }

        public List<Tour> GetAll()
        {
            return _tourRepository.GetAll();
        }

        public void Delete(Tour tour)
        {
            _tourRepository.Delete(tour);
        }

        public void DeleteById(int tourId)
        {
            _tourRepository.DeleteById(tourId);
        }

        public Tour GetByParentId(int parentId)
        {
            return _tourRepository.GetByParentId(parentId);
        }

        public Tour CreateTour(Tour tour)
        {
            tour.Images = _tourImageService.SaveAllByParentId(_tourRepository.NextId(), tour.Images);

            var savedTour = _tourRepository.Save(tour);
            return savedTour;
        }

        public List<Tour> FindFinishedTours()
        {
            return _tourRepository.GetAll().Where(tour =>
                _tourDateService.GetAll().Any(date =>
                    date.HasEnded && date.Tour.Id == tour.Id)).ToList();
        }

        public List<Tour> GetToursToday()
        {
            var dateOnly = GetSystemDateOnly();
            var toursToday = new List<Tour>();
            foreach (var tour in _tourRepository.GetAll())
            {
                foreach (var tourDate in _tourDateService.GetByParentId(tour.Id))
                {
                    DateTime? tourDateOnly = _tourDateService.ExtractTourDate(tourDate);

                    if (tourDateOnly.Equals(dateOnly) && !tourDate.HasEnded)
                    {
                        toursToday.Add(tour);
                    }
                }
            }
            return toursToday;
        }

        public bool ToursScheduledToday()
        {
            var exists = false;
            var systemDateOnly = GetSystemDateOnly();

            foreach (var tour in _tourRepository.GetAll())
            {
                tour.TourDates = _tourDateService.GetByParentId(tour.Id);
                if ((from date in tour.TourDates let tourDateOnly = _tourDateService.ExtractTourDate(date) where tourDateOnly == systemDateOnly && !date.HasEnded select date).Any())
                {
                    exists = true;
                }
            }

            return exists;
        }

        public static DateTime GetSystemDateOnly()
        {
            var dateTime = DateTime.Now;
            return dateTime.Date;
        }
    }
}
