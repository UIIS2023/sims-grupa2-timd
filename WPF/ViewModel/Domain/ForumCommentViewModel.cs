using SimsProject.Application.Interface;
using SimsProject.Domain.Model;

namespace SimsProject.WPF.ViewModel.Domain
{
    public class ForumCommentViewModel : ViewModelBase
    {
        private readonly ForumComment _comment;

        internal int Id => _comment.Id;
        public User Author { get; }
        public string LegitAuthorToolTip { get; }
        public string CanReportToolTip { get; }
        public string Comment => _comment.Comment;
        public bool IsAuthorValid => _comment.IsAuthorValid;
        public bool CanBeReported { get; set; }
        public bool IsNotReported { get; }
        public int? ReportCount { get; set; }
        public bool HasReports { get; set; }

        public ForumCommentViewModel(ForumComment comment, User user)
        {
            var commentService = Injector.CreateInstance<IForumCommentService>();
            var reportService = Injector.CreateInstance<IForumCommentReportService>();
            var userService = Injector.CreateInstance<IUserService>();
            _comment = commentService.GetById(comment.Id);
            Author = userService.GetById(_comment.Author.Id);

            IsNotReported = !commentService.IsReportedByUser(_comment.Id, user);
            ReportCount = _comment.ReportCount == 0 ? null : _comment.ReportCount;
            HasReports = _comment.ReportCount != 0;
            CanBeReported = !_comment.IsAuthorValid;
            if (IsAuthorValid)
            {
                LegitAuthorToolTip = Author.Type == UserType.Owner
                    ? "Comment from a property owner at this location"
                    : "Comment from a guest that has visited this location";
            }
            else
            {
                CanReportToolTip = IsNotReported ? "Report comment" : "You've already reported this comment";
            }
        }
    }
}
