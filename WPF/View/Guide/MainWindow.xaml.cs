using SimsProject.Domain.Model;
using SimsProject.WPF.View.Authentication;
using SimsProject.WPF.ViewModel.Guide;
using System;
using System.Windows;
using System.Windows.Input;

namespace SimsProject.WPF.View.Guide
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public MainWindow(User guide)
        {
            InitializeComponent();
            _mainWindowViewModel = new MainWindowViewModel(guide);

            MainWindowViewModel.ActionCompleted += OnActionCompleted;
            _mainWindowViewModel.ShowMessageBoxResult += ViewModel_ConfirmAction;
            _mainWindowViewModel.ShowMessageBox += ViewModel_ShowMessageBox;
            _mainWindowViewModel.LogoutRequested += OnLogoutRequested;
            _mainWindowViewModel.OpenWindowRequested += ViewModel_OpenWindowRequested;

            DataContext = _mainWindowViewModel;
        }

        private static void ViewModel_OpenWindowRequested(object sender, OpenWindowEventArgs e)
        {
            e.Window.Show();
        }

        private void ViewModel_ConfirmAction(object sender, string message)
        {
            const string caption = "Overview";
            const MessageBoxButton button = MessageBoxButton.YesNo;
            const MessageBoxImage icon = MessageBoxImage.Warning;

            var result = MessageBoxResult.None;

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                result = MessageBox.Show(message, caption, button, icon);
            });

            _mainWindowViewModel.ConfirmAction(result);
        }

        private static void ViewModel_ShowMessageBox(object sender, string message)
        {
            const string caption = "Overview";
            const MessageBoxButton button = MessageBoxButton.OK;
            const MessageBoxImage icon = MessageBoxImage.Warning;

            MessageBox.Show(message, caption, button, icon);
        }

        private void OnActionCompleted(object sender, ActionCompletedEventArgs e)
        {
            Close();
        }

        private void OnLogoutRequested(object sender, EventArgs e)
        {
            var loginForm = new LogInForm();
            loginForm.Show();

            Close();
        }

        private void ListViewItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (MenuButton.IsChecked == true)
            {
                TtTourRegistration.Visibility = Visibility.Collapsed;
                TtTourRequestStatistics.Visibility = Visibility.Collapsed;
                TtLiveTourTracking.Visibility = Visibility.Collapsed;
                TtMostVisitedTour.Visibility = Visibility.Collapsed;
                TtTourStatistics.Visibility = Visibility.Collapsed;
                TtTourReviews.Visibility = Visibility.Collapsed;
                TtTourRequests.Visibility = Visibility.Collapsed;
                TtTourRequestStatistics.Visibility = Visibility.Collapsed;
                TtLogOut.Visibility = Visibility.Collapsed;
                TtComplexTourRequest.Visibility = Visibility.Collapsed;
            }
            else
            {
                TtTourRegistration.Visibility = Visibility.Visible;
                TtTourRequestStatistics.Visibility = Visibility.Visible;
                TtLiveTourTracking.Visibility = Visibility.Visible;
                TtMostVisitedTour.Visibility = Visibility.Visible;
                TtTourStatistics.Visibility = Visibility.Visible;
                TtTourReviews.Visibility = Visibility.Visible;
                TtTourRequests.Visibility = Visibility.Visible;
                TtTourRequestStatistics.Visibility = Visibility.Visible;
                TtLogOut.Visibility = Visibility.Visible;
                TtComplexTourRequest.Visibility = Visibility.Visible;
            }
        }
    }
}
