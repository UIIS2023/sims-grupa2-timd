using SimsProject.WPF.ViewModel.Guide;
using System.Windows;
using SimsProject.Domain.Model;

namespace SimsProject.WPF.View.Guide
{
    /// <summary>
    /// Interaction logic for TourRequestStatistics.xaml
    /// </summary>
    public partial class TourRequestStatistics : Window
    {
        private readonly TourRequestStatisticsViewModel _tourRequestStatisticsViewModel;
        public User LoggedInUser { get; set; }

        public TourRequestStatistics(User user)
        {
            InitializeComponent();
            LoggedInUser = user;
            _tourRequestStatisticsViewModel = new TourRequestStatisticsViewModel();

            _tourRequestStatisticsViewModel.ShowMessageBox += ViewModel_ShowMessageBox;
            TourRequestStatisticsViewModel.ActionCompleted += OnActionCompleted;

            DataContext = _tourRequestStatisticsViewModel;
        }

        private static void ViewModel_ShowMessageBox(object sender, string message)
        {
            const MessageBoxButton button = MessageBoxButton.OK;
            const MessageBoxImage icon = MessageBoxImage.Warning;
            const string caption = "Tour request statistics";
            MessageBox.Show(message, caption, button, icon);
        }

        private void OnActionCompleted(object sender, ActionCompletedEventArgs e)
        {
            MainWindow mainWindow = new(LoggedInUser);
            mainWindow.Show();
            Close();
        }
    }
}
