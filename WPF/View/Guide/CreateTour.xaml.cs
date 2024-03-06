using System.Windows;
using SimsProject.Domain.Model;
using SimsProject.WPF.ViewModel.Domain;
using SimsProject.WPF.ViewModel.Guide;

namespace SimsProject.WPF.View.Guide
{
    /// <summary>
    /// Interaction logic for CreateTour.xaml
    /// </summary>
    public partial class CreateTour : Window
    {

        private readonly CreateTourViewModel _createTourViewModel;
        public User LoggedInUser { get; set; }

        public CreateTour(User user, TourRequestViewModel tourRequest, Location location, string language)
        {
            InitializeComponent();
            LoggedInUser = user;
            _createTourViewModel = new CreateTourViewModel(user, tourRequest, location, language);

            _createTourViewModel.ShowMessageBox += ViewModel_ShowMessageBox;
            _createTourViewModel.ShowMessageBoxResult += ViewModel_ConfirmAction;
            CreateTourViewModel.ActionCompleted += OnActionCompleted;

            DataContext = _createTourViewModel;
        }

        private static void ViewModel_ShowMessageBox(object sender, string message)
        {
            const string caption = "Tour creation";
            const MessageBoxButton button = MessageBoxButton.OK;

            MessageBox.Show(message, caption, button);
        }

        private void ViewModel_ConfirmAction(object sender, string message)
        {
            const string caption = "Tour creation";
            const MessageBoxButton button = MessageBoxButton.OKCancel;
            const MessageBoxImage icon = MessageBoxImage.Warning;

            var result = MessageBoxResult.None;

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                result = MessageBox.Show(message, caption, button, icon);
            });

            _createTourViewModel.ConfirmAction(result);
        }

        private void OnActionCompleted(object sender, ActionCompletedEventArgs e)
        {
            MainWindow mainWindow = new(LoggedInUser);
            mainWindow.Show();
            Close();
        }
    }
}