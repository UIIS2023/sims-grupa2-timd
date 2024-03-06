using SimsProject.Domain.RepositoryInterface;
using SimsProject.Domain.Model;
using SimsProject.Application.Interface;
using System.Collections.Generic;
using System.Linq;

namespace SimsProject.Application.UseCase
{
    public class ForumService : IForumService
    {
        private readonly IForumRepository _forumRepository;
        private readonly IForumCommentService _forumCommentService;
        private readonly IUserService _userService;

        public ForumService()
        {
            _forumRepository = Injector.CreateInstance<IForumRepository>();
            _forumCommentService = Injector.CreateInstance<IForumCommentService>();
            _userService = Injector.CreateInstance<IUserService>();
        }

        public Forum GetById(int id)
        {
            return _forumRepository.GetById(id);
        }

        public List<Forum> GetAll()
        {
            return _forumRepository.GetAll();
        }

        public void CloseForum(int forumId)
        {
            var forum = GetById(forumId);
            forum.IsClosed = true;
            _forumRepository.Update(forum);
        }

        public void Create(Forum forum)
        {
            _forumRepository.Save(forum);
        }

        public int GetCommentCount(int forumId)
        {
            return _forumCommentService.GetByForum(forumId).Count;
        }

        public bool IsUseful(int forumId)
        {
            var guestOnLocation = 0;
            var ownerOnLocation = 0;
            foreach (var comment in _forumCommentService.GetByForum(forumId).Where(comment => comment.IsAuthorValid))
            {
                if (_userService.GetById(comment.Author.Id).Type == UserType.Owner)
                {
                    ownerOnLocation++;
                }
                else guestOnLocation++;
            }
            // 2,1 instead of 20,10 for testing purposes
            return guestOnLocation >= 2 && ownerOnLocation >= 1;
        }
    }
}
