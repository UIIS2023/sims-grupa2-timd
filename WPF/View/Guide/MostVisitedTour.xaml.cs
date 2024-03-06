using System.Windows;
using SimsProject.Domain.Model;
using SimsProject.WPF.ViewModel.Guide;

namespace SimsProject.WPF.View.Guide
{
    /// <summary>
    /// Interaction logic for MostVisitedTour.xaml
    /// </summary>
    public partial class MostVisitedTour : Window
    {
        private readonly MostVisitedTourViewModel _mostVisitedTourViewModel;
        public User LoggedInUser { get; set; }

        public MostVisitedTour(User user)
        {
            InitializeComponent();
            LoggedInUser = user;
            _mostVisitedTourViewModel = new MostVisitedTourViewModel(user);

            _mostVisitedTourViewModel.ShowMessageBox += ViewModel_ShowMessageBox;
            MostVisitedTourViewModel.ActionCompleted += OnActionCompleted;

            DataContext = _mostVisitedTourViewModel;
        }

        private static void ViewModel_ShowMessageBox(object sender, string message)
        {
            const string caption = "Most visited tour";
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
