using SimsProject.WPF.ViewModel.Guest1;
using System.Windows.Controls;
using SimsProject.WPF.ViewModel;

namespace SimsProject.WPF.View.Guest1
{
    public partial class ReviewAccommodationForm : Page
    {
        public ReviewAccommodationForm()
        {
            InitializeComponent();
            Gb.Focus();
            var appViewModel = (AppViewModel)System.Windows.Application.Current.Resources["AppViewModel"];
            DataContext = new ReviewAccommodationViewModel(appViewModel.LoggedInUser, appViewModel.NavigationService);
        }
    }
}
