using SimsProject.WPF.ViewModel;
using SimsProject.WPF.ViewModel.Owner;
using System.Windows.Controls;

namespace SimsProject.WPF.View.Owner
{
    /// <summary>
    /// Interaction logic for ReviewsPanel.xaml
    /// </summary>
    public partial class ReviewsPanel : Page
    {
        public ReviewsPanel()
        {
            InitializeComponent();

            var appViewModel = (AppViewModel)System.Windows.Application.Current.Resources["AppViewModel"];
            DataContext = new ReviewsPanelViewModel(appViewModel.LoggedInUser);
        }
    }
}
