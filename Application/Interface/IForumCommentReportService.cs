using System.Collections.Generic;
using SimsProject.Domain.Model;

namespace SimsProject.Application.Interface
{
    public interface IForumCommentReportService : IService<ForumCommentReport>
    {
        List<ForumCommentReport> GetByComment(int forumId);
        ForumCommentReport CreateReport(ForumCommentReport report);
    }
}
