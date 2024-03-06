using System.Windows.Controls;
using SimsProject.WPF.ViewModel;
using SimsProject.WPF.ViewModel.Guest1;

namespace SimsProject.WPF.View.Guest1
{
    public partial class ReserveAccommodationForm : Page
    {
        public ReserveAccommodationForm()
        {
            InitializeComponent();
            Dp.Focus();
            var appViewModel = (AppViewModel)System.Windows.Application.Current.Resources["AppViewModel"];
            DataContext = new ReserveAccommodationViewModel(appViewModel.LoggedInUser, appViewModel.NavigationService);
        }
    }
}
