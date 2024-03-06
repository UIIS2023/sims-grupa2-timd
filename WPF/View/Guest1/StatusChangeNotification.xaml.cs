using SimsProject.WPF.ViewModel;
using SimsProject.WPF.ViewModel.Guest1;
using System.Windows;

namespace SimsProject.WPF.View.Guest1
{
    public partial class StatusChangeNotification : Window
    {
        public StatusChangeNotification()
        {
            InitializeComponent();
            var appViewModel = (AppViewModel)System.Windows.Application.Current.Resources["AppViewModel"];
            DataContext = new StatusChangeNotificationViewModel(this, appViewModel.NavigationService);
        }
    }
}
