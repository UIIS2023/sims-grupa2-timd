using SimsProject.Domain.Model;

namespace SimsProject.Application.Interface
{
    public interface IGuest1Service
    {
        void DeductPoint(User user);
        User CheckSuperGuest(User user);
        int GetNumberOfReservations(User currentUser);
        int GetNumberOfReservationsPastYear(User currentUser);
        int GetNumberOfReservationsPastPeriod(User currentUser);
        double GetAverageCleanliness(User currentUser);
        double GetAverageObservance(User currentUser);
    }
}
