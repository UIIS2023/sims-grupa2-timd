using BespokeFusion;
using SimsProject.WPF.ViewModel;
using SimsProject.WPF.ViewModel.Owner;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SimsProject.WPF.View.Owner
{
    /// <summary>
    /// Interaction logic for ProfilePanel.xaml
    /// </summary>
    public partial class ProfilePanel : Page
    {
        private readonly ProfilePanelViewModel _viewModel;

        public ProfilePanel()
        {
            InitializeComponent();

            var appViewModel = (AppViewModel)System.Windows.Application.Current.Resources["AppViewModel"];

            _viewModel = new ProfilePanelViewModel(appViewModel.LoggedInUser, appViewModel.NavigationService);
            _viewModel.SuperOwnerRequested += OnSuperOwnerRequested;
            _viewModel.SuccessRequested += OnSuccessRequested;
            _viewModel.FailRequested += OnFailRequested;

            DataContext = _viewModel;
        }

        private void CurrentPasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is not null)
            {
                _viewModel.CurrentPassword = ((PasswordBox)sender).Password;
            }
        }

        private void NewPasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is not null)
            {
                _viewModel.NewPassword = ((PasswordBox)sender).Password;
            }
        }

        private void OnSuperOwnerRequested(object sender, EventArgs e)
        {
            var caption = "✨Super-Owner";
            var text = "If you see a star next to your profile photo, it means you've qualified as a Super-Owner. " +
                       "This means that you've received at least 50 guest reviews with an average rating of 9.5 or higher. " +
                       "As a result, your accommodations will appear more prominently in search results, and guests will see a star next to them as well";

            var messageBox = new CustomMaterialMessageBox
            {
                VerticalContentAlignment = VerticalAlignment.Stretch,
                BtnOk = {
                    Content = "Got it!", Width = 150,
                    VerticalAlignment = VerticalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Background = Brushes.MediumSeaGreen, Foreground = Brushes.Black, BorderBrush = Brushes.SeaGreen
                },
                BtnCancel = { Visibility = Visibility.Hidden },

                Width = 400,
                Height = 255,
                TxtMessage = { Text = text, Foreground = Brushes.Black },
                TxtTitle = { Text = caption, Foreground = Brushes.Black },
                MainContentControl = { Background = Brushes.DarkSeaGreen },
                TitleBackgroundPanel = { Background = Brushes.SeaGreen },
                BorderBrush = Brushes.SeaGreen,
                BtnCopyMessage = { Visibility = Visibility.Hidden }
            };

            messageBox.Show();
        }

        private void OnSuccessRequested(object sender, EventArgs e)
        {
            var caption = "Password Change";
            var text = "You've successfully changed you password!";

            var messageBox = new CustomMaterialMessageBox
            {
                VerticalContentAlignment = VerticalAlignment.Stretch,
                BtnOk = {
                    Content = "Ok", Width = 150,
                    VerticalAlignment = VerticalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Background = Brushes.MediumSeaGreen, Foreground = Brushes.Black, BorderBrush = Brushes.SeaGreen
                },
                BtnCancel = { Visibility = Visibility.Hidden },

                Width = 400,
                Height = 250,
                TxtMessage = { Text = text, Foreground = Brushes.Black },
                TxtTitle = { Text = caption, Foreground = Brushes.Black },
                MainContentControl = { Background = Brushes.DarkSeaGreen },
                TitleBackgroundPanel = { Background = Brushes.SeaGreen },
                BorderBrush = Brushes.SeaGreen,
                BtnCopyMessage = { Visibility = Visibility.Hidden }
            };

            messageBox.Show();
        }

        private void OnFailRequested(object sender, EventArgs e)
        {
            var caption = "Password Change";
            var text = "Wrong current password, try again!";

            var messageBox = new CustomMaterialMessageBox
            {
                VerticalContentAlignment = VerticalAlignment.Stretch,
                BtnOk = {
                    Content = "Ok", Width = 150,
                    VerticalAlignment = VerticalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Background = Brushes.MediumSeaGreen, Foreground = Brushes.Black, BorderBrush = Brushes.SeaGreen

                },
                BtnCancel = { Visibility = Visibility.Hidden },

                Width = 400,
                Height = 250,
                TxtMessage = { Text = text, Foreground = Brushes.Black },
                TxtTitle = { Text = caption, Foreground = Brushes.Black },
                MainContentControl = { Background = Brushes.DarkSeaGreen },
                TitleBackgroundPanel = { Background = Brushes.SeaGreen },
                BorderBrush = Brushes.SeaGreen,
                BtnCopyMessage = { Visibility = Visibility.Hidden }
            };

            messageBox.Show();
        }
    }
}