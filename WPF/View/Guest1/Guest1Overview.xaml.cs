using System;
using System.Windows;
using System.Windows.Threading;
using SimsProject.WPF.View.Authentication;
using SimsProject.WPF.ViewModel;
using SimsProject.WPF.ViewModel.Guest1;

namespace SimsProject.WPF.View.Guest1
{
    public partial class Guest1Overview : Window
    {
        public Guest1Overview()
        {
            InitializeComponent();
            var app = System.Windows.Application.Current;
            var appViewModel = (AppViewModel)app.Resources["AppViewModel"];
            appViewModel.NavigationService = Panel.NavigationService;

            var mainWindowViewModel = new Guest1OverviewViewModel(appViewModel.LoggedInUser, appViewModel.NavigationService);
            mainWindowViewModel.LogoutRequested += OnLogoutRequested;
            DataContext = mainWindowViewModel;

            Uri uri = new Uri("Resources/Styles/Guest1StyleDark.xaml", UriKind.Relative);
            appViewModel.Theme = new ResourceDictionary() { Source = uri };
            app.Resources.MergedDictionaries.Add(appViewModel.Theme);
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(mainWindowViewModel.Notify));
        }
        private void OnLogoutRequested(object sender, EventArgs e)
        {
            var loginForm = new LogInForm();
            loginForm.Show();

            var app = System.Windows.Application.Current;
            var appViewModel = (AppViewModel)app.Resources["AppViewModel"];
            app.Resources.MergedDictionaries.Remove(appViewModel.Theme);

            Close();
        }
    }
}
