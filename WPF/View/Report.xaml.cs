using SimsProject.WPF.ViewModel;
using System;
using System.IO;
using System.Threading;
using System.Windows;

namespace SimsProject.WPF.View
{
    /// <summary>
    /// Interaction logic for Report.xaml
    /// </summary>
    public partial class Report : Window
    {
        private readonly string _temporaryFilePath;

        public Report(string temporaryFilePath)
        {
            InitializeComponent();

            var app = System.Windows.Application.Current;
            var appViewModel = (AppViewModel)app.Resources["AppViewModel"];
            Owner = appViewModel.MainWindow;

            _temporaryFilePath = temporaryFilePath;

            // PdfWebBrowser.Navigate("about:blank");
            PdfWebBrowser.Navigate(_temporaryFilePath);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            const int delayMilliseconds = 100;

            Thread.Sleep(delayMilliseconds);

            for (var i = 0; i < 3; i++)
            {
                try
                {
                    File.Delete(_temporaryFilePath);
                    break;
                }
                catch (IOException)
                {
                    Thread.Sleep(delayMilliseconds);
                }
            }
        }
    }
}
