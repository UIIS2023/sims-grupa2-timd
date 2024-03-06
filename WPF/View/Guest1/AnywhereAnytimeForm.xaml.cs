using System.Windows.Controls;
using SimsProject.WPF.ViewModel;
using SimsProject.WPF.ViewModel.Guest1;

namespace SimsProject.WPF.View.Guest1
{
    public partial class AnywhereAnytimeForm : Page
    {
        public AnywhereAnytimeForm()
        {
            InitializeComponent();
            Tb.Focus();
            var appViewModel = (AppViewModel)System.Windows.Application.Current.Resources["AppViewModel"];
            DataContext = new AnywhereAnytimeViewModel(appViewModel.LoggedInUser, appViewModel.NavigationService);
        }
    }
}
