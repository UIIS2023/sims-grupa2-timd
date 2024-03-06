using SimsProject.WPF.ViewModel.Guest1;
using SimsProject.WPF.ViewModel;
using System.Windows.Controls;

namespace SimsProject.WPF.View.Guest1
{
    public partial class CreateNewForumForm : Page
    {
        public CreateNewForumForm()
        {
            InitializeComponent();
            Cb.Focus();
            var appViewModel = (AppViewModel)System.Windows.Application.Current.Resources["AppViewModel"];
            DataContext = new CreateNewForumViewModel(appViewModel.LoggedInUser, appViewModel.NavigationService);
        }
    }
}
