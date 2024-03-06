using SimsProject.WPF.ViewModel.Guide;
using System.Windows;
using User = SimsProject.Domain.Model.User;

namespace SimsProject.WPF.View.Guide
{
    /// <summary>
    /// Interaction logic for TourStatistics.xaml
    /// </summary>
    public partial class TourStatistics : Window
    {
        private readonly TourStatisticsViewModel _tourStatisticsViewModel;
        public User LoggedInUser { get; set; }

        public TourStatistics(User user)
        {
            InitializeComponent();
            LoggedInUser = user;
            _tourStatisticsViewModel = new TourStatisticsViewModel(user);

            _tourStatisticsViewModel.ShowMessageBox += ViewModel_ShowMessageBox;
            TourStatisticsViewModel.ActionCompleted += OnActionCompleted;
            _tourStatisticsViewModel.OpenReportRequested += OnOpenReportRequested;

            DataContext = _tourStatisticsViewModel;

        }
        private static void OnOpenReportRequested(object sender, string filePath)
        {
            var dialog = new Report(filePath);
            dialog.ShowDialog();
        }

        private static void ViewModel_ShowMessageBox(object sender, string message)
        {
            const string caption = "Tours statistics";
            const MessageBoxButton button = MessageBoxButton.OK;
            const MessageBoxImage icon = MessageBoxImage.Warning;
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
