using System.Collections.ObjectModel;
using System.Windows.Navigation;
using System.Windows.Input;
using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.WPF.ViewModel.Domain;

namespace SimsProject.WPF.ViewModel.Guest1
{
    public class SelectedForumViewModel : ViewModelBase
    {
        public User CurrentUser;
        public NavigationService NavigationService { get; set; }
        public string LocationText { get; set; }
        public string PostText { get; set; }
        public ObservableCollection<ForumCommentViewModel> Comments { get; set; }
        public ForumViewModel SelectedForum { get; set; }
        private readonly IForumCommentService _forumCommentService;
        private readonly IForumService _forumService;
        private readonly ILocationService _locationService;
        private readonly IUserService _userService;
        public SelectedForumViewModel(User currentUser, NavigationService navigationService)
        {
            CurrentUser = currentUser;
            NavigationService = navigationService;
            SelectedForum = ForumsViewModel.SelectedForum;
            LocationText = SelectedForum.Location.ToString();
            PostText = SelectedForum.TitlePost;
            _forumCommentService = Injector.CreateInstance<IForumCommentService>();
            _forumService = Injector.CreateInstance<IForumService>();
            _locationService = Injector.CreateInstance<ILocationService>();
            _userService = Injector.CreateInstance<IUserService>();
            Comments = new ObservableCollection<ForumCommentViewModel>();
            CommentCommand = new RelayCommand(CommentClick, CanCommentClick);
            GoBackCommand = new RelayCommand(GoBackClick);
            GetComments();
        }
        public ICommand GoBackCommand { get; set; }
        public void GoBackClick(object obj)
        {
            NavigationService.GoBack();
        }
        void GetComments()
        {
            Comments.Clear();
            foreach (var forumComment in _forumCommentService.GetByForum(SelectedForum.Id))
            {
                Comments.Add(new ForumCommentViewModel(forumComment, CurrentUser));
            }
        }
        public ICommand CommentCommand { get; set; }

        public bool CanCommentClick(object obj)
        {
            return !SelectedForum.IsClosed && !string.IsNullOrEmpty(Text);
        }
        public void CommentClick(object obj)
        {
            bool was = _userService.IsAuthorValid(CurrentUser, _locationService.GetByCityAndCountry(SelectedForum.Location.City, SelectedForum.Location.Country).Id);
            ForumComment forumComment = new ForumComment(_forumService.GetById(SelectedForum.Id), CurrentUser, Text, was);
            _forumCommentService.CreateComment(forumComment);
            Text = "";
            GetComments();
        }

        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }
    }
}
