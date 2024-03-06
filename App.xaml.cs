using SimsProject.Application.Localization;
using SimsProject.WPF.ViewModel;
using System.Windows;

namespace SimsProject
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Injector.Initialize();

            var appViewModel = new AppViewModel();
            Resources.Add("AppViewModel", appViewModel);
        }

        public void ChangeLanguage(string lang)
        {
            TranslationSource.Instance.CurrentCulture = new System.Globalization.CultureInfo(lang);
        }
    }
}
