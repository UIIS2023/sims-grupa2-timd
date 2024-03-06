using SimsProject.Domain.RepositoryInterface;
using SimsProject.Domain.Model;
using SimsProject.Application.Interface;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.Design;

namespace SimsProject.Application.UseCase
{
    public class ForumCommentReportService : IForumCommentReportService
    {
        private readonly IForumCommentReportRepository _reportRepository;

        public ForumCommentReportService()
        {
            _reportRepository = Injector.CreateInstance<IForumCommentReportRepository>();
        }

        public ForumCommentReport GetById(int id)
        {
            return _reportRepository.GetById(id);
        }

        public List<ForumCommentReport> GetByComment(int commentId)
        {
            return _reportRepository.GetAll()
                                    .Where(comment => comment.Comment.Id == commentId)
                                    .ToList();
        }

        public ForumCommentReport CreateReport(ForumCommentReport report)
        {
            return _reportRepository.Save(report);
        }
    }
}
