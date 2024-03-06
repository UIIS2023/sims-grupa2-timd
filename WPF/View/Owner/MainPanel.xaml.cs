using System.Windows.Controls;
using SimsProject.WPF.ViewModel;
using SimsProject.WPF.ViewModel.Owner;

namespace SimsProject.WPF.View.Owner
{
    /// <summary>
    /// Interaction logic for MainPanel.xaml
    /// </summary>
    public partial class MainPanel : Page
    {
        public MainPanel()
        {
            InitializeComponent();

            var appViewModel = (AppViewModel)System.Windows.Application.Current.Resources["AppViewModel"];
            DataContext = new MainPanelViewModel(appViewModel.LoggedInUser, appViewModel.NavigationService);
        }
    }
}
