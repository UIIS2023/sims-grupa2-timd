using SimsProject.Application.Interface;
using SimsProject.WPF.ViewModel.Domain;
using System;
using System.Collections.ObjectModel;

using System.Windows.Input;
using SimsProject.Domain.Model;

namespace SimsProject.WPF.ViewModel.Owner
{
    public class ForumPanelViewModel : ViewModelBase
    {
        public User Owner { get; set; }
        public ObservableCollection<ForumViewModel> Forums { get; set; }
        public ObservableCollection<ForumCommentViewModel> Comments { get; set; }

        #region SERVICES

        private IUserService _userService;
        private IForumService _forumService;
        private IForumCommentService _commentService;

        #endregion

        #region PROPERTIES

        private ForumViewModel _selectedForum;
        public ForumViewModel SelectedForum
        {
            get => _selectedForum;
            set
            {
                if (value != _selectedForum)
                {
                    _selectedForum = value;
                    RefreshForum();
                    OnPropertyChanged(nameof(SelectedForum));
                    OnPropertyChanged(nameof(IsForumSelected));
                    OnPropertyChanged(nameof(IsCommentingAllowed));
                }
            }
        }

        private bool _isForumSelected;
        public bool IsForumSelected
        {
            get => _isForumSelected;
            set
            {
                if (value != _isForumSelected)
                {
                    _isForumSelected = value;
                    OnPropertyChanged(nameof(IsForumSelected));
                }
            }
        }

        private bool _isCommentingAllowed;
        public bool IsCommentingAllowed
        {
            get => _isCommentingAllowed;
            set
            {
                if (value != _isCommentingAllowed)
                {
                    _isCommentingAllowed = value;
                    OnPropertyChanged(nameof(IsCommentingAllowed));
                }
            }
        }

        private string _comment;
        public string Comment
        {
            get => _comment;
            set
            {
                if (value != _comment)
                {
                    _comment = value;
                    OnPropertyChanged(nameof(Comment));
                }
            }
        }

        #endregion

        #region COMMANDS

        public ICommand PostCommand { get; set; }
        public ICommand ReportCommand { get; set; }

        #endregion

        #region EVENTS

        public bool ConfirmActionResult { get; set; }
        public event EventHandler ConfirmActionRequested;

        #endregion

        public ForumPanelViewModel(User owner)
        {
            Owner = owner;

            InitializeServices();
            InitializeCollections();
            InitializeCommands();
        }

        private void InitializeServices()
        {
            _userService = Injector.CreateInstance<IUserService>();
            _forumService = Injector.CreateInstance<IForumService>();
            _commentService = Injector.CreateInstance<IForumCommentService>();
        }

        private void InitializeCollections()
        { 
            Forums = new ObservableCollection<ForumViewModel>();
            foreach (var forum in _forumService.GetAll())
            {
                Forums.Add(new ForumViewModel(forum));
            }

            Comments = new ObservableCollection<ForumCommentViewModel>();
        }

        private void InitializeCommands()
        {
            PostCommand = new RelayCommand(Post, CanPost);
            ReportCommand = new RelayCommand(Report);
        }

        public void RefreshForum()
        {
            if (SelectedForum is null) return;

            IsForumSelected = true;
            var hasAccommodationOnLocation = _userService.IsAuthorValid(Owner, SelectedForum.Location.Id);

            IsCommentingAllowed = !SelectedForum.IsClosed && hasAccommodationOnLocation;
            
            Comments.Clear();
            foreach (var comment in _commentService.GetByForum(SelectedForum.Id))
            {
                Comments.Add(new ForumCommentViewModel(comment, Owner));
            }

            if (IsCommentingAllowed) return;
            foreach (var comment in Comments)
            {
                comment.CanBeReported = false;
            }
        }

        private void Post(object obj)
        {
            var forum = _forumService.GetById(SelectedForum.Id);
            var newComment = new ForumComment(forum, Owner, Comment, true);
            var addedComment = _commentService.CreateComment(newComment);

            Comments.Add(new ForumCommentViewModel(addedComment, Owner));
            Comment = null;
        }

        private bool CanPost(object obj)
        {
            return !string.IsNullOrWhiteSpace(Comment);
        }

        private void Report(object obj)
        {
            if (obj is not ForumCommentViewModel comment) return;

            ConfirmActionRequested?.Invoke(this, EventArgs.Empty);
            if (!ConfirmActionResult) return;

            var reportedComment = _commentService.ReportComment(comment.Id, Owner);
            RefreshForum();
        }
    }
}
