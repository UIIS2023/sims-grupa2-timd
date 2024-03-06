using System.Windows;
using SimsProject.WPF.ViewModel.Guide;
using User = SimsProject.Domain.Model.User;

namespace SimsProject.WPF.View.Guide
{
    /// <summary>
    /// Interaction logic for TourReviews.xaml
    /// </summary>
    public partial class TourReviews : Window
    {
        private readonly TourReviewsViewModel _tourReviewsViewModel;
        public User LoggedInUser { get; set; }

        public TourReviews(User user)
        {
            InitializeComponent();
            LoggedInUser = user;
            _tourReviewsViewModel = new TourReviewsViewModel(user);

            _tourReviewsViewModel.ShowMessageBox += ViewModel_ShowMessageBox;
            _tourReviewsViewModel.ShowMessageBoxResult += ViewModel_ConfirmAction;
            TourReviewsViewModel.ActionCompleted += OnActionCompleted;

            DataContext = _tourReviewsViewModel;
        }

        private static void ViewModel_ShowMessageBox(object sender, string message)
        {
            const string caption = "Tours reviews";
            const MessageBoxButton button = MessageBoxButton.OK;

            MessageBox.Show(message, caption, button);
        }

        private void ViewModel_ConfirmAction(object sender, string message)
        {
            const string caption = "Tours reviews";
            const MessageBoxButton button = MessageBoxButton.OKCancel;
            const MessageBoxImage icon = MessageBoxImage.Warning;

            var result = MessageBoxResult.None;

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                result = MessageBox.Show(message, caption, button, icon);
            });

            _tourReviewsViewModel.ConfirmAction(result);
        }

        private void OnActionCompleted(object sender, ActionCompletedEventArgs e)
        {
            MainWindow mainWindow = new(LoggedInUser);
            mainWindow.Show();
            Close();
        }
    }
}

