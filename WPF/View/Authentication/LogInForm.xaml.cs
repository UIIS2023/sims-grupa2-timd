using System.Windows;
using System.Windows.Controls;
using SimsProject.WPF.ViewModel.Authentication;

namespace SimsProject.WPF.View.Authentication
{
    /// <summary>
    /// Interaction logic for LogInForm.xaml
    /// </summary>
    public partial class LogInForm : Window
    {
        private readonly LogInFormViewModel _viewModel;

        public LogInForm()
        {
            InitializeComponent();
            _viewModel = new LogInFormViewModel(this);
            DataContext = _viewModel;
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                _viewModel.Password = passwordBox.Password;
            }
        }
    }
}
