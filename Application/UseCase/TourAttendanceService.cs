using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.Domain.RepositoryInterface;
using System.Collections.Generic;
using System.Linq;

namespace SimsProject.Application.UseCase
{
    public class TourAttendanceService : ITourAttendanceService
    {
        private readonly ITourAttendanceRepository _tourAttendanceRepository;
        private readonly ICheckPointService _checkPointService;

        public TourAttendanceService()
        {
            _tourAttendanceRepository = Injector.CreateInstance<ITourAttendanceRepository>();
            _checkPointService = Injector.CreateInstance<ICheckPointService>();
        }

        public TourAttendance GetById(int id)
        {
            return _tourAttendanceRepository.GetById(id);
        }

        public List<TourAttendance> GetAll()
        {
            return _tourAttendanceRepository.GetAll();
        }

        public void Update(TourAttendance notification) {

            _tourAttendanceRepository.Update(notification);
        }

        public void DeleteAllByParentId(int parentId)
        {
            _tourAttendanceRepository.DeleteAllByParentId(parentId);
        }

        public TourAttendance GetByGuest(User guest)
        {
            return _tourAttendanceRepository.GetByGuest(guest);
        }

        public TourAttendance Save(TourAttendance tourAttendance)
        {
            return _tourAttendanceRepository.Save(tourAttendance);
        }

        public CheckPoint GetMyCheckPoint(User guest) 
        {
            var point = new CheckPoint();
            foreach (var attendance in _tourAttendanceRepository.GetAll().Where(attendance => attendance.User.Id == guest.Id && (int)(attendance.Present) == 2))
            {
                point = _checkPointService.GetById(attendance.CheckPoint.Id);
            }
            return point;
        }

        public CheckPoint GetCheckPoint(User guest, Tour tour)
        {
            var point = new CheckPoint();
            foreach (var attendance in _tourAttendanceRepository.GetAll().Where(attendance => attendance.User.Id == guest.Id && (int)(attendance.Present) == 2 && tour.Id == attendance.Tour.Id))
            {
                point = _checkPointService.GetById(attendance.CheckPoint.Id);
            }
            return point;
        }
    }
}