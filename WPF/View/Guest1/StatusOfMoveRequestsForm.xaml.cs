using System.Windows.Controls;
using SimsProject.WPF.ViewModel;
using SimsProject.WPF.ViewModel.Guest1;

namespace SimsProject.WPF.View.Guest1
{
    public partial class StatusOfMoveRequestsForm : Page
    {
        public StatusOfMoveRequestsForm()
        {
            InitializeComponent();
            Dg.Focus();
            var appViewModel = (AppViewModel)System.Windows.Application.Current.Resources["AppViewModel"];
            DataContext = new StatusOfMoveRequestsFormViewModel(appViewModel.LoggedInUser, appViewModel.NavigationService);
        }
    }
}
