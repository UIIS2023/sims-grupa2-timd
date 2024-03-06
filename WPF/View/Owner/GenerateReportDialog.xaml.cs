using SimsProject.WPF.ViewModel.Owner;
using System;
using System.Windows;

namespace SimsProject.WPF.View.Owner
{
    /// <summary>
    /// Interaction logic for GenerateReportDialog.xaml
    /// </summary>
    public partial class GenerateReportDialog : Window
    {
        public GenerateReportDialog(GenerateReportDialogViewModel generateReportDialogViewModel)
        {
            InitializeComponent();

            generateReportDialogViewModel.CloseDialogRequested += OnCloseDialogRequested;
            
            DataContext = generateReportDialogViewModel;
        }

        private void OnCloseDialogRequested(object sender, EventArgs e)
        {
            Close();
        }
    }
}
