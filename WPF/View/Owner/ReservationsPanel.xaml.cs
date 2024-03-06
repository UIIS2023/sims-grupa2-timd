using BespokeFusion;
using SimsProject.WPF.ViewModel;
using SimsProject.WPF.ViewModel.Owner;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace SimsProject.WPF.View.Owner
{
    /// <summary>
    /// Interaction logic for ReservationsPanel.xaml
    /// </summary>
    public partial class ReservationsPanel : Page
    {
        private readonly ReservationsPanelViewModel _viewModel;

        public ReservationsPanel()
        {
            InitializeComponent();

            var appViewModel = (AppViewModel)System.Windows.Application.Current.Resources["AppViewModel"];
            
            _viewModel = new ReservationsPanelViewModel(appViewModel.LoggedInUser);
            _viewModel.ConfirmActionRequested += OnConfirmActionRequested;
            _viewModel.RejectActionRequested += OnRejectActionRequested;

            DataContext = _viewModel;
        }

        private void OnConfirmActionRequested(object sender, EventArgs e)
        {
            var caption = "Reschedule request";
            var text = $"Are you sure you want to reschedule {_viewModel.SelectedRequest.Guest.Username}'s request?\n" +
                       "If there are any overlapping reservations, they will be canceled";

            var messageBox = new CustomMaterialMessageBox
            {
                BtnOk = { Content = "Reschedule", Width = 150, Background = Brushes.MediumSeaGreen, Foreground = Brushes.Black, BorderBrush = Brushes.SeaGreen },
                BtnCancel = { Content = "Cancel", Width = 150, Foreground = Brushes.Black, BorderBrush = Brushes.SeaGreen },

                Width = 400,
                Height = 250,
                TxtMessage = { Text = text, Foreground = Brushes.Black },
                TxtTitle = { Text = caption, Foreground = Brushes.Black },
                MainContentControl = { Background = Brushes.DarkSeaGreen },
                TitleBackgroundPanel = { Background = Brushes.SeaGreen },
                BorderBrush = Brushes.SeaGreen,
                BtnCopyMessage = { Visibility = Visibility.Hidden }
            };

            messageBox.ShowDialog();
            _viewModel.ConfirmActionResult = messageBox.Result == MessageBoxResult.OK;
        }

        private void OnRejectActionRequested(object sender, EventArgs e)
        {
            var dialogViewModel = new RejectRescheduleDialogViewModel();
            var dialog = new RejectRescheduleDialog(dialogViewModel);
            dialog.ShowDialog();

            if (!dialogViewModel.Result)
            { 
                _viewModel.RejectActionResult = false;
                return;
            }

            _viewModel.RejectActionResult = true;
            _viewModel.Explanation = dialogViewModel.Explanation;
        }
    }
}
