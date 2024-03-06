using SimsProject.WPF.ViewModel.Guide;
using System.Windows;
using SimsProject.Domain.Model;

namespace SimsProject.WPF.View.Guide
{
    /// <summary>
    /// Interaction logic for TourRequests.xaml
    /// </summary>
    public partial class TourRequests : Window
    {
        public User LoggedInUser { get; set; }
        private readonly TourRequestsViewModel _tourRequestsViewModel;

        public TourRequests(User user)
        {
            InitializeComponent();
            LoggedInUser = user;
            _tourRequestsViewModel = new TourRequestsViewModel(user);

            TourRequestsViewModel.ActionCompleted += OnActionCompleted;
            _tourRequestsViewModel.OpenWindowRequested += ViewModel_OpenWindowRequested;

            DataContext = _tourRequestsViewModel;
        }

        private static void ViewModel_OpenWindowRequested(object sender, OpenWindowEventArgs e)
        {
            e.Window.Show();
        }

        private void OnActionCompleted(object sender, ActionCompletedEventArgs e)
        {
            MainWindow mainWindow = new(LoggedInUser);
            mainWindow.Show();
            Close();
        }
    }
}
