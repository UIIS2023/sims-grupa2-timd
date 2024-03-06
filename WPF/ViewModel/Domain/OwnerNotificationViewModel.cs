using SimsProject.Application.Interface;
using SimsProject.Domain.Model;

namespace SimsProject.WPF.ViewModel.Domain
{
    public class OwnerNotificationViewModel : ViewModelBase
    {
        private readonly Notification _notification;

        internal int Id => _notification.Id;
        public User User { get; }
        public string Message => _notification.Message;
        internal NotificationType Type => _notification.Type;
        public string NotificationIconName
        {
            get
            {
                return Type switch
                {
                    NotificationType.Review => "UserClock",
                    NotificationType.Cancel => "CalendarTimes",
                    NotificationType.Reschedule => "ClockRotateLeft",
                    NotificationType.Forum => "MapPin",
                    _ => null,
                };
            }
        }

        public OwnerNotificationViewModel(INotificationService notificationService, Notification notification)
        {
            var userService = Injector.CreateInstance<IUserService>();
            _notification = notificationService.GetById(notification.Id);

            User = userService.GetById(_notification.User.Id);
        }
    }
}