using SimsProject.Application.Interface;
using SimsProject.Domain.Model;

namespace SimsProject.WPF.ViewModel.Domain
{
    public class ForumViewModel : ViewModelBase
    {
        private readonly Forum _forum;

        internal int Id => _forum.Id;
        public User Creator { get; }
        public string TitlePost => _forum.TitlePost + (_forum.IsClosed ? " [Closed]" : "");
        public LocationViewModel Location { get; }
        public bool IsClosed => _forum.IsClosed;
        public int CommentCount { get; }
        public bool IsUseful {get; }

        public ForumViewModel(Forum forum)
        {
            var forumService = Injector.CreateInstance<IForumService>();
            var userService = Injector.CreateInstance<IUserService>();
            _forum = forumService.GetById(forum.Id);

            Creator = userService.GetById(_forum.Creator.Id);
            Location = new LocationViewModel(_forum.Location);

            CommentCount = forumService.GetCommentCount(forum.Id);
            IsUseful = forumService.IsUseful(forum.Id);
        }
    }
}
