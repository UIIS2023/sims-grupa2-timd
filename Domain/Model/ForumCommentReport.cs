using System;
using SimsProject.Serializer;

namespace SimsProject.Domain.Model
{
    public class ForumCommentReport : ISerializable
    {
        public int Id { get; set; }
        public ForumComment Comment { get; set; }
        public User Reporter { get; set; }

        public ForumCommentReport() { }

        public ForumCommentReport(ForumComment comment, User reporter)
        {
            Comment = comment;
            Reporter = reporter;
        }

        public string[] ToCsv()
        {
            string[] csvValues = { Id.ToString(), Comment.Id.ToString(), Reporter.Id.ToString() };
            return csvValues;
        }

        public void FromCsv(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            Comment = new ForumComment() { Id = Convert.ToInt32(values[1]) };
            Reporter = new User() { Id = Convert.ToInt32(values[2]) };
        }
    }
}
