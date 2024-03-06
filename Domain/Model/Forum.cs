using System;
using SimsProject.Serializer;


namespace SimsProject.Domain.Model
{
    public class Forum : ISerializable
    {
        public int Id { get; set; }
        public User Creator { get; set; }
        public string TitlePost { get; set; }
        public Location Location { get; set; }
        public bool IsClosed { get; set; } 

        public Forum() { }

        public Forum(User creator, string post, Location location)
        {
            Creator = creator;
            TitlePost = post;
            Location = location;
            IsClosed = false;
        }

        public string[] ToCsv()
        {
            string[] csvValues = { Id.ToString(), Creator.Id.ToString(), TitlePost, Location.Id.ToString(), IsClosed.ToString()};
            return csvValues;
        }

        public void FromCsv(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            Creator = new User() { Id = Convert.ToInt32(values[1]) };
            TitlePost = values[2];
            Location = new Location() { Id = Convert.ToInt32(values[3]) };
            IsClosed = Convert.ToBoolean(values[4]);
        }
    }
}
