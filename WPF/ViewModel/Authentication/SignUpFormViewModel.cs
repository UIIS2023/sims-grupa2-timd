using SimsProject.Domain.Model;
using System.Windows;
using System.Windows.Input;
using SimsProject.Application.Interface;

namespace SimsProject.WPF.ViewModel.Authentication
{
    public class SignUpFormViewModel : ViewModelBase
    {
        private readonly IUserService _userService;
        private readonly Window _signUpWindow;

        #region COMMANDS

        public ICommand SignUpCommand { get; set; }

        #endregion

        #region PROPERTIES

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                if (value != _username)
                {
                    _username = value;
                    OnPropertyChanged(nameof(Username));
                }
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                if (value != _password)
                {
                    _password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }

        private int _selectedType;
        public int SelectedTypeIndex
        {
            get => _selectedType;
            set
            {
                if (value != _selectedType)
                {
                    _selectedType = value;
                    OnPropertyChanged(nameof(SelectedTypeIndex));
                }
            }
        }

        public string[] UserTypes { get; set; }

        #endregion

        public SignUpFormViewModel(Window window)
        {
            _signUpWindow = window;
            _userService = Injector.CreateInstance<IUserService>();

            SignUpCommand = new RelayCommand(SignUp);
            SetUserTypes();

        }

        private void SetUserTypes()
        {
            UserTypes = new[] { "Owner", "Guide", "Guest1", "Guest2" };
            SelectedTypeIndex = 0;
        }

        private void SignUp(object obj)
        {
            var newUser = new User(Username, Password, (UserType)SelectedTypeIndex);
            var savedUser = _userService.CreateUser(newUser);
            if (savedUser is not null)
            {
                MessageBox.Show("Sign up successful!");
                _signUpWindow.Close();
            }
            else
            {
                MessageBox.Show("Username already taken!");
            }
        }
    }
}
