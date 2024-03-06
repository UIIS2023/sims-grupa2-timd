using SimsProject.Domain.Model;
using System.Collections.Generic;

namespace SimsProject.Application.Interface
{
    public interface ITourAttendanceService : IService<TourAttendance>
    {
        List<TourAttendance> GetAll();
        void Update(TourAttendance notification);
        void DeleteAllByParentId(int parentId);
        TourAttendance GetByGuest(User guest);
        TourAttendance Save(TourAttendance tourAttendance);
        CheckPoint GetCheckPoint(User guest,Tour tour);
    }
}