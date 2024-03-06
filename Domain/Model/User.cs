using System;
using SimsProject.Serializer;

namespace SimsProject.Domain.Model
{
    public enum UserType
    {
        Owner, Guide, Guest1, Guest2
    }

    public class User : ISerializable
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public UserType Type { get; set; }
        public bool IsSuperUser { get; set; }
        public int? SuperUserPoints { get; set; }
        public DateOnly BecameSuperUser { get; set; }

        public User() { }

        public User(string username, string password, UserType type)
        {
            Username = username;
            Password = password;
            Type = type;
            IsSuperUser = false;
            if (type == UserType.Guest1)
            {
                SuperUserPoints = 0;
                BecameSuperUser = DateOnly.MinValue;
            }
        }

        public string[] ToCsv()
        {
            var temp = BecameSuperUser.ToString("dd.MM.yyyy.");
            if (Type != UserType.Guest1) temp = string.Empty;
            string[] csvValues = { Id.ToString(), Username, Password, ((int)Type).ToString(), IsSuperUser.ToString(), SuperUserPoints.ToString(), temp };
            return csvValues;
        }

        public void FromCsv(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            Username = values[1];
            Password = values[2];
            Type = (UserType)Convert.ToInt32(values[3]);
            IsSuperUser = Convert.ToBoolean(values[4]);
            SuperUserPoints = int.TryParse(values[5], out _) ? int.Parse(values[5]) : null;
            BecameSuperUser = DateOnly.TryParse(values[6], out _) ? DateOnly.Parse(values[6]) : DateOnly.MinValue;
        }
    }
}
