using SimsProject.Domain.Model;
using SimsProject.Serializer;
using System.Collections.Generic;
using System.Linq;
using SimsProject.Domain.RepositoryInterface;

namespace SimsProject.Repository
{
    public class ForumCommentReportCsvRepository : IForumCommentReportRepository
    {
        private const string FilePath = "../../../Resources/Data/forumCommentReports.csv";

        private readonly Serializer<ForumCommentReport> _serializer;

        private List<ForumCommentReport> _reports;

        public ForumCommentReportCsvRepository()
        {
            _serializer = new Serializer<ForumCommentReport>();
            _reports = _serializer.FromCsv(FilePath);
        }

        public List<ForumCommentReport> GetAll()
        {
            return _serializer.FromCsv(FilePath);
        }

        public ForumCommentReport GetById(int id)
        {
            _reports = _serializer.FromCsv(FilePath);
            return _reports.FirstOrDefault(r => r.Id == id);
        }

        public int NextId()
        {
            _reports = _serializer.FromCsv(FilePath);
            if (_reports.Count < 1)
            {
                return 1;
            }
            return _reports.Max(r => r.Id) + 1;
        }

        public ForumCommentReport Save(ForumCommentReport report)
        {
            report.Id = NextId();
            _reports = _serializer.FromCsv(FilePath);
            _reports.Add(report);
            _serializer.ToCsv(FilePath, _reports);
            return report;
        }

        public ForumCommentReport Update(ForumCommentReport report)
        {
            _reports = _serializer.FromCsv(FilePath);
            ForumCommentReport current = _reports.Find(r => r.Id == report.Id);
            int index = _reports.IndexOf(current);
            _reports.Remove(current);
            _reports.Insert(index, report);
            _serializer.ToCsv(FilePath, _reports);
            return report;
        }

        public void Delete(ForumCommentReport report)
        {
            _reports = _serializer.FromCsv(FilePath);
            ForumCommentReport found = _reports.Find(r => r.Id == report.Id);
            _reports.Remove(found);
            _serializer.ToCsv(FilePath, _reports);
        }
    }
}
