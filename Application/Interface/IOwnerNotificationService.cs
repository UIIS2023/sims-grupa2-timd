using System.Collections.Generic;
using SimsProject.Domain.Model;

namespace SimsProject.Application.Interface
{
    public interface IOwnerNotificationService : INotificationService
    {
        List<Notification> GetInboxByUser(User owner);   
        void ReadNotification(int notificationId);
    }
}
