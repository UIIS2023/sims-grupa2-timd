using System;
using System.Windows;
using System.Windows.Media;
using BespokeFusion;
using SimsProject.WPF.View.Authentication;
using SimsProject.WPF.ViewModel;
using SimsProject.WPF.ViewModel.Owner;

namespace SimsProject.WPF.View.Owner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            var app = System.Windows.Application.Current;
            var appViewModel = (AppViewModel)app.Resources["AppViewModel"];
            appViewModel.NavigationService = Panel.NavigationService;
            appViewModel.MainWindow = this;

            _viewModel = new MainWindowViewModel(appViewModel.LoggedInUser, appViewModel.NavigationService);
            _viewModel.ConfirmActionRequested += OnConfirmActionRequested;
            _viewModel.LogoutRequested += OnLogoutRequested;

            DataContext = _viewModel;
        }

        private void OnConfirmActionRequested(object sender, EventArgs e)
        {
            var caption = "Logout";
            var text = "Are you sure you want to logout?";

            var messageBox = new CustomMaterialMessageBox
            {
                BtnOk = { Content = "Yes", Width = 150, Background = Brushes.MediumSeaGreen, Foreground = Brushes.Black, BorderBrush = Brushes.SeaGreen },
                BtnCancel = { Content = "No", Width = 150, Foreground = Brushes.Black, BorderBrush = Brushes.SeaGreen },

                Width = 400,
                Height = 250,
                TxtMessage = { Text = text, Foreground = Brushes.Black },
                TxtTitle = { Text = caption, Foreground = Brushes.Black },
                MainContentControl = { Background = Brushes.DarkSeaGreen },
                TitleBackgroundPanel = { Background = Brushes.SeaGreen },
                BorderBrush = Brushes.SeaGreen,
                BtnCopyMessage = { Visibility = Visibility.Hidden }
            };

            messageBox.ShowDialog();
            _viewModel.ConfirmActionResult = messageBox.Result == MessageBoxResult.OK;
        }

        private void OnLogoutRequested(object sender, EventArgs e)
        {
            var loginForm = new LogInForm();
            loginForm.Show();

            Close();
        }
    }
}
