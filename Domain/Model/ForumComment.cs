using System;
using SimsProject.Serializer;


namespace SimsProject.Domain.Model
{
    public class ForumComment : ISerializable
    {
        public int Id { get; set; }
        public Forum Forum { get; set; }
        public User Author { get; set; }
        public string Comment { get; set; }
        public bool IsAuthorValid { get; set; }
        public int ReportCount { get; set; }

        public ForumComment() { }

        public ForumComment(Forum forum, User creator, string comment, bool isAuthorValid)
        {
            Forum = forum;
            Author = creator;
            Comment = comment;
            IsAuthorValid = isAuthorValid;
            ReportCount = 0;
        }

        public string[] ToCsv()
        {
            string[] csvValues = { Id.ToString(), Forum.Id.ToString(), Author.Id.ToString(), Comment, IsAuthorValid.ToString(), ReportCount.ToString() };
            return csvValues;
        }

        public void FromCsv(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            Forum = new Forum() { Id = Convert.ToInt32(values[1]) };
            Author = new User() { Id = Convert.ToInt32(values[2]) };
            Comment = values[3];
            IsAuthorValid = Convert.ToBoolean(values[4]);
            ReportCount = Convert.ToInt32(values[5]);
        }
    }
}
