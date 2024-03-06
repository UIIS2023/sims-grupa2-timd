using SimsProject.WPF.ViewModel;
using SimsProject.WPF.ViewModel.Guest1;
using System.Windows.Controls;

namespace SimsProject.WPF.View.Guest1
{
    public partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            InitializeComponent();
            var appViewModel = (AppViewModel)System.Windows.Application.Current.Resources["AppViewModel"];
            DataContext = new ProfilePageViewModel(appViewModel.LoggedInUser, appViewModel.NavigationService);
        }
    }
}
