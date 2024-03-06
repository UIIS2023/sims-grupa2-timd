using SimsProject.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimsProject.Application.Interface
{
    public interface  IGuest2NotificationService : INotificationService
    {
       
        public void ReadNotification(int notificationId);
        public void NotifyGuest2(Tour newTour,int requestId);
        
    }
}
