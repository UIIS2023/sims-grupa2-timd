using System.Windows.Controls;
using SimsProject.WPF.ViewModel;
using SimsProject.WPF.ViewModel.Guest1;

namespace SimsProject.WPF.View.Guest1
{
    public partial class ForumsForm : Page
    {
        public ForumsForm()
        {
            InitializeComponent();
            Bt.Focus();
            var appViewModel = (AppViewModel)System.Windows.Application.Current.Resources["AppViewModel"];
            DataContext = new ForumsViewModel(appViewModel.LoggedInUser, appViewModel.NavigationService);
        }
    }
}
