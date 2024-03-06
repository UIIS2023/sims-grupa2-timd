using System.Collections.Generic;
using SimsProject.Domain.Model;

namespace SimsProject.Application.Interface
{
    public interface ITourService : IService<Tour>
    {
        List<Tour> GetByGuide(User guide);
        List<Tour> GetAll();
        void Delete(Tour tour);
        void DeleteById(int tourId);
        Tour GetByParentId(int parentId);
        Tour CreateTour(Tour tour);
        List<Tour> FindFinishedTours(); 
        List<Tour> GetToursToday();
        bool ToursScheduledToday();
    }
}
