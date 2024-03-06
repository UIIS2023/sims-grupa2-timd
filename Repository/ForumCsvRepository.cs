using SimsProject.Domain.Model;
using SimsProject.Serializer;
using System.Collections.Generic;
using System.Linq;
using SimsProject.Domain.RepositoryInterface;

namespace SimsProject.Repository
{
    public class ForumCsvRepository : IForumRepository
    {
        private const string FilePath = "../../../Resources/Data/forums.csv";

        private readonly Serializer<Forum> _serializer;

        private List<Forum> _forums;

        public ForumCsvRepository()
        {
            _serializer = new Serializer<Forum>();
            _forums= _serializer.FromCsv(FilePath);
        }

        public List<Forum> GetAll()
        {
            return _serializer.FromCsv(FilePath);
        }

        public Forum GetById(int id)
        {
            _forums = _serializer.FromCsv(FilePath);
            return _forums.FirstOrDefault(t => t.Id == id);
        }

        public int NextId()
        {
            _forums = _serializer.FromCsv(FilePath);
            if (_forums.Count < 1)
            {
                return 1;
            }
            return _forums.Max(t => t.Id) + 1;
        }

        public Forum Save(Forum forum)
        {
            forum.Id = NextId();
            _forums = _serializer.FromCsv(FilePath);
            _forums.Add(forum);
            _serializer.ToCsv(FilePath, _forums);
            return forum;
        }

        public Forum Update(Forum forum)
        {
            _forums = _serializer.FromCsv(FilePath);
            Forum current = _forums.Find(t => t.Id == forum.Id);
            int index = _forums.IndexOf(current);
            _forums.Remove(current);
            _forums.Insert(index, forum);
            _serializer.ToCsv(FilePath, _forums);
            return forum;
        }

        public void Delete(Forum forum)
        {
            _forums = _serializer.FromCsv(FilePath);
            Forum found = _forums.Find(t => t.Id == forum.Id);
            _forums.Remove(found);
            _serializer.ToCsv(FilePath, _forums);
        }
    }
}
