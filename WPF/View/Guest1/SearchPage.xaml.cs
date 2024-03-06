using SimsProject.WPF.ViewModel.Guest1;
using SimsProject.WPF.ViewModel;
using System.Windows.Controls;

namespace SimsProject.WPF.View.Guest1
{
    public partial class SearchPage : Page
    {
        public SearchPage()
        {
            InitializeComponent();
            Tb.Focus();
            var appViewModel = (AppViewModel)System.Windows.Application.Current.Resources["AppViewModel"];
            DataContext = new SearchPageViewModel(appViewModel.LoggedInUser, appViewModel.NavigationService);
        }
    }
}
