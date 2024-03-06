using System.Windows;
using SimsProject.Domain.Model;
using SimsProject.WPF.ViewModel.Guide;

namespace SimsProject.WPF.View.Guide
{
    /// <summary>
    /// Interaction logic for LiveTourTracking.xaml
    /// </summary>
    public partial class LiveTourTracking : Window
    {
        public User LoggedInUser { get; set; }

        public LiveTourTracking(User user)
        {
            InitializeComponent();
            LoggedInUser = user;

            LiveTourTrackingViewModel.ActionCompleted += OnActionCompleted;

            DataContext = new LiveTourTrackingViewModel(user);
        }

        private void OnActionCompleted(object sender, ActionCompletedEventArgs e)
        {
            MainWindow mainWindow = new(LoggedInUser);
            mainWindow.Show();
            Close();
        }
    }
}
