using System.Windows;
using SimsProject.WPF.ViewModel.Domain;
using SimsProject.WPF.ViewModel.Guide;
using User = SimsProject.Domain.Model.User;

namespace SimsProject.WPF.View.Guide
{
    /// <summary>
    /// Interaction logic for CancelTour.xaml
    /// </summary>
    public partial class CancelTour : Window
    {
        private readonly CancelTourViewModel _cancelTourViewModel;
        public User LoggedInUser { get; set; }
        
        public CancelTour(TourViewModel tour, User user)
        {
            InitializeComponent();
            LoggedInUser = user;
            _cancelTourViewModel = new CancelTourViewModel(tour, user);

            _cancelTourViewModel.ShowMessageBox += ViewModel_ShowMessageBox;
            _cancelTourViewModel.ShowMessageBoxResult += ViewModel_ConfirmAction;
            CancelTourViewModel.ActionCompleted += OnActionCompleted;

            DataContext = _cancelTourViewModel;
        }

        private static void ViewModel_ShowMessageBox(object sender, string message)
        {
            const string caption = "Tour cancellation";
            const MessageBoxButton button = MessageBoxButton.OK;
            const MessageBoxImage icon = MessageBoxImage.Warning;

            MessageBox.Show(message, caption, button, icon);
        }

        private void ViewModel_ConfirmAction(object sender, string message)
        {
            const string caption = "Tour cancellation";
            const MessageBoxButton button = MessageBoxButton.OKCancel;
            const MessageBoxImage icon = MessageBoxImage.Warning;

            var result = MessageBoxResult.None;

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                result = MessageBox.Show(message, caption, button, icon);
            });

            _cancelTourViewModel.ConfirmAction(result);
        }

        private void OnActionCompleted(object sender, ActionCompletedEventArgs e)
        {
            MainWindow mainWindow = new(LoggedInUser);
            mainWindow.Show();
            Close();
        }
    }
}
