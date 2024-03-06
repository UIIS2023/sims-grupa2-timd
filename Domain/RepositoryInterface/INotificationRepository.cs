using System.Collections.Generic;
using SimsProject.Domain.Model;

namespace SimsProject.Domain.RepositoryInterface
{
    public interface INotificationRepository : IRepository<Notification>
    {
        List<Notification> GetByUser(User user);
        List<Notification> GetUnreadByUser(User user);
    }
}