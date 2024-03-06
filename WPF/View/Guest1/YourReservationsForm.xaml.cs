using System.Windows.Controls;
using SimsProject.WPF.ViewModel;
using SimsProject.WPF.ViewModel.Guest1;

namespace SimsProject.WPF.View.Guest1
{
    public partial class YourReservationsForm : Page
    {
        public YourReservationsForm()
        {
            InitializeComponent();
            Dg.Focus();
            var appViewModel = (AppViewModel)System.Windows.Application.Current.Resources["AppViewModel"];
            DataContext = new YourReservationsFormViewModel(appViewModel.LoggedInUser, appViewModel.NavigationService);
        }
    }
}
