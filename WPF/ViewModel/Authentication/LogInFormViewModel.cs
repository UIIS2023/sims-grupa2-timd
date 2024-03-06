using System.Windows;
using System.Windows.Input;
using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.WPF.View.Authentication;

namespace SimsProject.WPF.ViewModel.Authentication
{
    public class LogInFormViewModel : ViewModelBase
    {
        private readonly IUserService _userService;
        private readonly Window _logInWindow;

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

        #endregion

        #region COMMANDS

        public ICommand LogInCommand { get; set; }
        public ICommand SignUpCommand { get; set; }

        #endregion

        public LogInFormViewModel(Window window)
        {
            _logInWindow = window;
            _userService = Injector.CreateInstance<IUserService>();

            _userService.UpdateAllSuperUserStatuses();

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            LogInCommand = new RelayCommand(LogIn, CanLogIn);
            SignUpCommand = new RelayCommand(SignUp);
        }

        private void LogIn(object obj)
        {
            var user = _userService.GetByUsername(Username);
            if (user is not null)
            {
                if (user.Password == Password)
                {
                    SetLoggedInUser(user);

                    OpenOverview(user);
                    _logInWindow.Close();
                }
                else
                {
                    MessageBox.Show("Wrong password!");
                }
            }
            else
            {
                MessageBox.Show("User not found!");
            }
        }

        private static void SetLoggedInUser(User user)
        {
            var app = System.Windows.Application.Current;
            var appViewModel = (AppViewModel)app.Resources["AppViewModel"];
            appViewModel.LoggedInUser = user;
        }

        private bool CanLogIn(object obj)
        {
            return !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);
        }

        private static void OpenOverview(User user)
        {
            Window overview;

            switch (user.Type)
            {
                case UserType.Owner:
                    overview = new View.Owner.MainWindow();
                    break;
                case UserType.Guide:
                    overview = new View.Guide.MainWindow(user);
                    break;
                case UserType.Guest1:
                    overview = new View.Guest1.Guest1Overview();
                    break;
                case UserType.Guest2:
                    overview = new View.Guest2.MainWindow(user);
                    break;
                default:
                    MessageBox.Show("Invalid user type.");
                    return;
            }

            overview.Show();
        }

        private static void SignUp(object obj)
        {
            SignUpForm signUpForm = new();
            signUpForm.ShowDialog();
        }
    }
}