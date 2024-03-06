using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.Domain.RepositoryInterface;
using System;
using System.Collections.Generic;

namespace SimsProject.Application.UseCase
{
    public class TourDateService : ITourDateService
    {
        private readonly ITourDateRepository _tourDateRepository;

        public TourDateService()
        {
            _tourDateRepository = Injector.CreateInstance<ITourDateRepository>();
        }

        public TourDate GetById(int id)
        {
            return _tourDateRepository.GetById(id);
        }

        public List<TourDate> GetAll()
        {
            return _tourDateRepository.GetAll();
        }

        public void DeleteAllByParentId(int parentId)
        {
            _tourDateRepository.DeleteAllByParentId(parentId);
        }

        public List<TourDate> GetByParentId(int parentId)
        {
            return _tourDateRepository.GetByParentId(parentId);
        }

        public TourDate Save(TourDate tourDate)
        {
            return _tourDateRepository.Save(tourDate);
        }

        public void Delete(TourDate tourDate)
        {
            _tourDateRepository.Delete(tourDate);
        }

        public TourDate Update(TourDate tourDate)
        {
            return _tourDateRepository.Update(tourDate);
        }

        public List<string> FindDistinctYears()
        {
            return _tourDateRepository.FindDistinctYears();
        }

        public TimeSpan CalculateTimeDifference(DateTime date)
        {
            var currentDate = DateTime.Now;
            var selectedDate = GetSelectedDate(date);
            var selectedDateTime = selectedDate + selectedDate.TimeOfDay;

            return selectedDateTime - currentDate;
        }

        public TourDate UpdateById(int tourDateId)
        {
            return _tourDateRepository.UpdateById(tourDateId);
        }

        public DateTime ExtractTourDate(TourDate tourDate)
        {
            DateTime? date = tourDate.Date;
            return date.Value.Date;
        }

        private static DateTime GetSelectedDate(DateTime date)
        {
            return date.Date;
        }
    }
}
