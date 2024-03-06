using SimsProject.Application.Interface;
using SimsProject.Domain.Model;

namespace SimsProject.WPF.ViewModel.Domain
{
    public class UserViewModel : ViewModelBase
    {
        private readonly User _user;

        internal int Id => _user.Id;
        public string Username => _user.Username;

        public UserViewModel(User user)
        {
            var userService = Injector.CreateInstance<IUserService>();
            _user = userService.GetById(user.Id);
        }
    }
}
