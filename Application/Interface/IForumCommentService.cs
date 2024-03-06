using System.Collections.Generic;
using SimsProject.Domain.Model;

namespace SimsProject.Application.Interface
{
    public interface IForumCommentService : IService<ForumComment>
    {
        List<ForumComment> GetByForum(int forumId);
        ForumComment CreateComment(ForumComment comment);
        ForumComment ReportComment(int commentId, User reporter);
        bool IsReportedByUser(int commentId, User user);
    }
}
