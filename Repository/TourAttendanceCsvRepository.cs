using SimsProject.Domain.Model;
using SimsProject.Domain.RepositoryInterface;
using SimsProject.Serializer;
using System.Collections.Generic;
using System.Linq;
using User = SimsProject.Domain.Model.User;

namespace SimsProject.Repository
{
    public class TourAttendanceCsvRepository : ITourAttendanceRepository
    {
        private const string FilePath = "../../../Resources/Data/tourAttendances.csv";

        private readonly Serializer<TourAttendance> _serializer;
        private List<TourAttendance> _tourAttendances;

        public TourAttendanceCsvRepository()
        {
            _serializer = new Serializer<TourAttendance>();
            _tourAttendances = _serializer.FromCsv(FilePath);
        }

        public List<TourAttendance> GetAll()
        {
            var attendances = _serializer.FromCsv(FilePath);
            return attendances;
        }

        public int NextId()
        {
            _tourAttendances = GetAll();
            if (_tourAttendances.Count < 1)
            {
                return 1;
            }
            return _tourAttendances.Max(c => c.Id) + 1;
        }

        public TourAttendance Update(TourAttendance attendance) 
        {
            _tourAttendances = GetAll();
            var currentAttendance = _tourAttendances.Find(n => n.Id == attendance.Id);

            currentAttendance.Present = attendance.Present;
            currentAttendance.CheckPoint = attendance.CheckPoint;
            var index = _tourAttendances.IndexOf(currentAttendance);

            _tourAttendances.Remove(currentAttendance);
            _tourAttendances.Insert(index, currentAttendance);
            _serializer.ToCsv(FilePath, _tourAttendances);
            return currentAttendance;
        }

        public void Delete(TourAttendance tourAttendance) 
        {
            _tourAttendances = GetAll();
            _tourAttendances.Remove(tourAttendance);
            _serializer.ToCsv(FilePath, _tourAttendances);
        }

        public TourAttendance Save(TourAttendance tourAttendance) 
        {
            tourAttendance.Id = NextId();
            _tourAttendances = _serializer.FromCsv(FilePath);
            _tourAttendances.Add(tourAttendance);
            _serializer.ToCsv(FilePath, _tourAttendances);
            return tourAttendance;
        }
        
        public TourAttendance GetById(int id) 
        { 
            return _tourAttendances.Find(n => n.Id == id);
        }

        public TourAttendance GetByGuest(User guest)
        {
            _tourAttendances = GetAll();
            return _tourAttendances.FirstOrDefault(n => n.User.Id == guest.Id);
        }

        public void DeleteAllByParentId(int parentId)
        {
            _tourAttendances = _serializer.FromCsv(FilePath);
            var found = _tourAttendances.FindAll(i => i.Tour.Id == parentId);
            foreach (var tourAttendance in found)
            {
                _tourAttendances.Remove(tourAttendance);
            }
            _serializer.ToCsv(FilePath, _tourAttendances);
        }
    }
}