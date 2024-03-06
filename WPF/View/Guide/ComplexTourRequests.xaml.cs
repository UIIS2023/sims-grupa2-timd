using SimsProject.Domain.Model;
using SimsProject.WPF.ViewModel.Guide;
using System.Windows;

namespace SimsProject.WPF.View.Guide
{
    /// <summary>
    /// Interaction logic for ComplexTourRequests.xaml
    /// </summary>
    public partial class ComplexTourRequests : Window
    {
        public User LoggedInUser { get; set; }
        private readonly ComplexTourRequestsViewModel _complexTourRequestsViewModel;

        public ComplexTourRequests(User user)
        {
            InitializeComponent();
            LoggedInUser = user;
            _complexTourRequestsViewModel = new ComplexTourRequestsViewModel(user);

            ComplexTourRequestsViewModel.ActionCompleted += OnActionCompleted;
            _complexTourRequestsViewModel.OpenWindowRequested += ViewModel_OpenWindowRequested;

            DataContext = _complexTourRequestsViewModel;
        }

        private static void ViewModel_OpenWindowRequested(object sender, OpenWindowEventArgs e)
        {
            e.Window.Show();
        }

        private void OnActionCompleted(object sender, ActionCompletedEventArgs e)
        {
            Close();
        }
    }
}
