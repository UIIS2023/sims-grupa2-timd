using System.Collections.Generic;
using SimsProject.Domain.Model;

namespace SimsProject.Application.Interface
{
    public interface INotificationService : IService<Notification>
    {
        List<Notification> GetByUser(User user);
    }
}
