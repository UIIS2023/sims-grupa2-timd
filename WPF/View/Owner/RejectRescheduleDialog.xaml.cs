using System;
using System.Windows;
using SimsProject.WPF.ViewModel.Owner;

namespace SimsProject.WPF.View.Owner
{
    /// <summary>
    /// Interaction logic for RejectRescheduleDialog.xaml
    /// </summary>
    public partial class RejectRescheduleDialog : Window
    {
        public RejectRescheduleDialog(RejectRescheduleDialogViewModel rejectRescheduleDialogViewModel)
        {
            InitializeComponent();

            rejectRescheduleDialogViewModel.CloseDialogRequested += OnCloseDialogRequested;

            DataContext = rejectRescheduleDialogViewModel;
        }

        private void OnCloseDialogRequested(object sender, EventArgs e)
        {
            Close();
        }
    }
}
