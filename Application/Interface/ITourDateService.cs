using SimsProject.Domain.Model;
using System;
using System.Collections.Generic;

namespace SimsProject.Application.Interface
{
    public interface ITourDateService : IService<TourDate>
    {
        List<TourDate> GetByParentId(int parentId);
        List<TourDate> GetAll();
        List<string> FindDistinctYears();
        void DeleteAllByParentId(int parentId);
        TourDate Save(TourDate tourDate);
        TourDate Update(TourDate tourDate);
        TourDate UpdateById(int tourDateId);
        DateTime ExtractTourDate(TourDate tourDate);
        TimeSpan CalculateTimeDifference(DateTime date);
        void Delete(TourDate tourDate);
    }
}
