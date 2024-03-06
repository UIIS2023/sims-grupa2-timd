using System.Collections.Generic;
using System.Linq;
using SimsProject.Domain.Model;
using SimsProject.Domain.RepositoryInterface;
using SimsProject.Serializer;

namespace SimsProject.Repository
{
    public class NotificationCsvRepository : INotificationRepository
    {
        private const string FilePath = "../../../Resources/Data/notifications.csv";

        private readonly Serializer<Notification> _serializer;

        private List<Notification> _notifications;

        public NotificationCsvRepository()
        {
            _serializer = new Serializer<Notification>();
            _notifications = _serializer.FromCsv(FilePath);
        }

        public List<Notification> GetAll()
        {
            return _serializer.FromCsv(FilePath);
        }

        public Notification GetById(int id)
        {
            _notifications = _serializer.FromCsv(FilePath);
            return _notifications.FirstOrDefault(n => n.Id == id);
        }

        public int NextId()
        {
            _notifications = _serializer.FromCsv(FilePath);
            if (_notifications.Count < 1)
            {
                return 1;
            }
            return _notifications.Max(u => u.Id) + 1;
        }

        public Notification Save(Notification notification)
        {
            notification.Id = NextId();
            _notifications = _serializer.FromCsv(FilePath);
            _notifications.Add(notification);
            _serializer.ToCsv(FilePath, _notifications);
            return notification;
        }

        public Notification Update(Notification notification)
        {
            _notifications = _serializer.FromCsv(FilePath);
            Notification current = _notifications.Find(n => n.Id == notification.Id);
            int index = _notifications.IndexOf(current);
            _notifications.Remove(current);
            _notifications.Insert(index, notification);
            _serializer.ToCsv(FilePath, _notifications);
            return notification;
        }

        public void Delete(Notification notification)
        {
            _notifications = _serializer.FromCsv(FilePath);
            Notification found = _notifications.Find(n => n.Id == notification.Id);
            _notifications.Remove(found);
            _serializer.ToCsv(FilePath, _notifications);
        }

        public List<Notification> GetByUser(User user)
        {
            _notifications = _serializer.FromCsv(FilePath);
            var notificationsOrdered = _notifications.FindAll(n => n.User.Id == user.Id);
            notificationsOrdered.Reverse();
            return notificationsOrdered;
        }

        public List<Notification> GetUnreadByUser(User user)
        {
            _notifications = _serializer.FromCsv(FilePath);
            var notificationsOrdered = _notifications.FindAll(n => n.User.Id == user.Id && !n.IsRead);
            notificationsOrdered.Reverse();
            return notificationsOrdered;
        }
    }
}
