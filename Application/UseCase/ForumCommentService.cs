using SimsProject.Domain.RepositoryInterface;
using SimsProject.Domain.Model;
using SimsProject.Application.Interface;
using System.Collections.Generic;
using System.Linq;

namespace SimsProject.Application.UseCase
{
    public class ForumCommentService : IForumCommentService
    {
        private readonly IForumCommentRepository _commentRepository;
        private readonly IForumCommentReportService _reportService;

        public ForumCommentService()
        {
            _commentRepository = Injector.CreateInstance<IForumCommentRepository>();
            _reportService = Injector.CreateInstance<IForumCommentReportService>();
        }

        public ForumComment GetById(int id)
        {
            return _commentRepository.GetById(id);
        }

        public List<ForumComment> GetByForum(int forumId)
        {
            return _commentRepository.GetAll()
                                     .Where(comment => comment.Forum.Id == forumId)
                                     .ToList();
        }

        public ForumComment CreateComment(ForumComment comment)
        {
            return _commentRepository.Save(comment);
        }

        public ForumComment ReportComment(int commentId, User reporter)
        {
            var comment = _commentRepository.GetById(commentId);

            var report = new ForumCommentReport(comment, reporter);
            _reportService.CreateReport(report);

            comment.ReportCount++;
            return _commentRepository.Update(comment);
        }

        public bool IsReportedByUser(int commentId, User user)
        {
            return _reportService.GetByComment(commentId).Any(report => report.Reporter.Id == user.Id);
        }
    }
}
