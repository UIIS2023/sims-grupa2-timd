using System.Collections.Generic;
using SimsProject.Domain.Model;

namespace SimsProject.Application.Interface
{
    public interface IForumService : IService<Forum>
    {
        List<Forum> GetAll();
        void CloseForum(int forumId);
        void Create(Forum forum);
        int GetCommentCount(int forumId);
        bool IsUseful(int forumId);
    }
}
