using System;
using SimsProject.Serializer;

namespace SimsProject.Domain.Model
{
    public enum NotificationType
    {
        Review,
        Cancel,
        Reschedule,
        Forum,
        Request
    };

    public class Notification : ISerializable
    {
        public int Id { get; set; }
        public User User { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; }

        public Notification() { }

        public Notification(User user, string message, NotificationType type)
        {
            User = user;
            Message = message;
            Type = type;
            IsRead = false;
        }

        public string[] ToCsv()
        {
            string[] csvValues = { Id.ToString(), User.Id.ToString(), Message, Type.ToString(), IsRead.ToString() };
            return csvValues;
        }

        public void FromCsv(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            User = new User() { Id = Convert.ToInt32(values[1]) };
            Message = values[2];
            Type = Enum.Parse<NotificationType>(values[3]);
            IsRead = Convert.ToBoolean(values[4]);
        }
    }
}
