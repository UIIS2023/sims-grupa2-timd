using SimsProject.Domain.Model;

namespace SimsProject.Domain.RepositoryInterface
{
    public interface ITourAttendanceRepository : IRepository<TourAttendance>
    {
        TourAttendance GetByGuest(User guest);
        void DeleteAllByParentId(int parentId);
    }
}