using SimsProject.WPF.ViewModel.Guest1;
using SimsProject.WPF.ViewModel;
using System.Windows.Controls;

namespace SimsProject.WPF.View.Guest1
{
    public partial class SelectedForumForm : Page
    {
        public SelectedForumForm()
        {
            InitializeComponent();
            Tb.Focus();
            var appViewModel = (AppViewModel)System.Windows.Application.Current.Resources["AppViewModel"];
            DataContext = new SelectedForumViewModel(appViewModel.LoggedInUser, appViewModel.NavigationService);
        }
    }
}
