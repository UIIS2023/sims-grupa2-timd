using SimsProject.WPF.ViewModel;
using System;
using System.Windows;

namespace SimsProject.Localization
{
    class AppTheme
    {
        public static void ChangeTheme(Uri uri)
        {
            var app = System.Windows.Application.Current;
            var appViewModel = (AppViewModel)app.Resources["AppViewModel"];
            app.Resources.MergedDictionaries.Remove(appViewModel.Theme);
            appViewModel.Theme = new ResourceDictionary() { Source = uri };
            app.Resources.MergedDictionaries.Add(appViewModel.Theme);
        }
    }
}