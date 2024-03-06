using SimsProject.Domain.Model;

namespace SimsProject.Application.Interface
{
    public interface IGuideService
    {
        bool GetSuperGuideStatus(User guide);
        void Resign(User guide);
    }
}
