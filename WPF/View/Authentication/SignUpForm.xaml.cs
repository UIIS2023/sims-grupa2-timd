using System.Windows;
using System.Windows.Controls;
using SimsProject.WPF.ViewModel.Authentication;

namespace SimsProject.WPF.View.Authentication
{
    /// <summary>
    /// Interaction logic for SignUpForm.xaml
    /// </summary>
    public partial class SignUpForm : Window
    {
        private readonly SignUpFormViewModel _viewModel;

        public SignUpForm()
        {
            InitializeComponent();
            _viewModel = new SignUpFormViewModel(this);
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
