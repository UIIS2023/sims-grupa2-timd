using System.Windows.Controls;
using SimsProject.WPF.ViewModel;
using SimsProject.WPF.ViewModel.Guest1;

namespace SimsProject.WPF.View.Guest1
{
    public partial class ReviewsForm : Page
    {
        public ReviewsForm()
        {
            InitializeComponent();
            Dg.Focus();
            var appViewModel = (AppViewModel)System.Windows.Application.Current.Resources["AppViewModel"];
            DataContext = new ReviewsFormViewModel(appViewModel.LoggedInUser, appViewModel.NavigationService);
        }
    }
}
