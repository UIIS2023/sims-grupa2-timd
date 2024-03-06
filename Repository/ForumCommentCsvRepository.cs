using SimsProject.Domain.Model;
using SimsProject.Serializer;
using System.Collections.Generic;
using System.Linq;
using SimsProject.Domain.RepositoryInterface;

namespace SimsProject.Repository
{
    public class ForumCommentCsvRepository : IForumCommentRepository
    {
        private const string FilePath = "../../../Resources/Data/forumComments.csv";

        private readonly Serializer<ForumComment> _serializer;

        private List<ForumComment> _forumComments;

        public ForumCommentCsvRepository()
        {
            _serializer = new Serializer<ForumComment>();
            _forumComments = _serializer.FromCsv(FilePath);
        }

        public List<ForumComment> GetAll()
        {
            return _serializer.FromCsv(FilePath);
        }

        public ForumComment GetById(int id)
        {
            _forumComments = _serializer.FromCsv(FilePath);
            return _forumComments.FirstOrDefault(t => t.Id == id);
        }

        public int NextId()
        {
            _forumComments = _serializer.FromCsv(FilePath);
            if (_forumComments.Count < 1)
            {
                return 1;
            }
            return _forumComments.Max(t => t.Id) + 1;
        }

        public ForumComment Save(ForumComment forumComment)
        {
            forumComment.Id = NextId();
            _forumComments = _serializer.FromCsv(FilePath);
            _forumComments.Add(forumComment);
            _serializer.ToCsv(FilePath, _forumComments);
            return forumComment;
        }

        public ForumComment Update(ForumComment forumComment)
        {
            _forumComments = _serializer.FromCsv(FilePath);
            ForumComment current = _forumComments.Find(t => t.Id == forumComment.Id);
            int index = _forumComments.IndexOf(current);
            _forumComments.Remove(current);
            _forumComments.Insert(index, forumComment);
            _serializer.ToCsv(FilePath, _forumComments);
            return forumComment;
        }

        public void Delete(ForumComment forumComment)
        {
            _forumComments = _serializer.FromCsv(FilePath);
            ForumComment found = _forumComments.Find(t => t.Id == forumComment.Id);
            _forumComments.Remove(found);
            _serializer.ToCsv(FilePath, _forumComments);
        }
    }
}
