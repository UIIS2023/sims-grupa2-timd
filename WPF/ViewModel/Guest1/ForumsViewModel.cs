using SimsProject.Domain.Model;
using System.Windows.Input;
using System;
using System.Windows.Navigation;
using SimsProject.WPF.ViewModel.Domain;
using System.Collections.ObjectModel;
using SimsProject.Application.Interface;

namespace SimsProject.WPF.ViewModel.Guest1
{
    public class ForumsViewModel : ViewModelBase
    {
        public User CurrentUser;
        public NavigationService NavigationService { get; set; }
        public ObservableCollection<ForumViewModel> Forums { get; set; }
        public static ForumViewModel SelectedForum { get; set; }
        private readonly IForumService _forumService;
        public ForumsViewModel(User currentUser, NavigationService navigationService)
        {
            CurrentUser = currentUser;
            NavigationService = navigationService;
            SelectedForum = null;
            _forumService = Injector.CreateInstance<IForumService>();
            Forums = new ObservableCollection<ForumViewModel>();
            FillTable();
            CreateForumCommand = new RelayCommand(CreateForumClick);
            OpenForumCommand = new RelayCommand(OpenForumClick, CanOpenForumClick);
            DeleteForumCommand = new RelayCommand(DeleteForumClick, CanDeleteForumClick);
        }
        public void FillTable()
        {
            Forums.Clear();
            foreach (var forum in _forumService.GetAll())
            {
                Forums.Add(new ForumViewModel(forum));
            }
        }
        public ICommand CreateForumCommand { get; set; }
        public void CreateForumClick(object obj)
        {
            NavigationService.Navigate(
                new Uri("/SimsProject;component/WPF/View/Guest1/CreateNewForumForm.xaml", UriKind.Relative));
        }
        public ICommand OpenForumCommand { get; set; }
        public void OpenForumClick(object obj)
        {
            NavigationService.Navigate(
                new Uri("/SimsProject;component/WPF/View/Guest1/SelectedForumForm.xaml", UriKind.Relative));
        }
        public bool CanOpenForumClick(object obj)
        {
            return SelectedForum != null;
        }
        public ICommand DeleteForumCommand { get; set; }
        public void DeleteForumClick(object obj)
        {
            _forumService.CloseForum(SelectedForum.Id);
            FillTable();
        }
        public bool CanDeleteForumClick(object obj)
        {
            return SelectedForum != null && SelectedForum.Creator.Id == CurrentUser.Id && !SelectedForum.IsClosed;
        }
    }
}
