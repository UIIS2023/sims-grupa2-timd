using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using BespokeFusion;
using SimsProject.WPF.ViewModel;
using SimsProject.WPF.ViewModel.Owner;
using MessageBox = System.Windows.Forms.MessageBox;

namespace SimsProject.WPF.View.Owner
{
    /// <summary>
    /// Interaction logic for RenovationsPanel.xaml
    /// </summary>
    public partial class RenovationsPanel : Page
    {
        private readonly RenovationsPanelViewModel _viewModel;

        public RenovationsPanel()
        {
            InitializeComponent();

            var appViewModel = (AppViewModel)System.Windows.Application.Current.Resources["AppViewModel"];
            
            _viewModel = new RenovationsPanelViewModel(appViewModel.LoggedInUser);
            _viewModel.ConfirmActionRequested += OnConfirmActionRequested;
            _viewModel.GenerateReportRequested += OnGenerateReportRequested;
            _viewModel.OpenReportRequested += OnOpenReportRequested;

            DataContext = _viewModel;
        }

        private void OnConfirmActionRequested(object sender, EventArgs e)
        {
            var caption = "Cancel renovation";
            var text = $"If you are sure you want to cancel the renovation of {_viewModel.SelectedRenovation.Accommodation.Name}, click 'Yes'.\n" +
                       "If you change your mind and want to schedule another renovation, simply return to the scheduling section of the renovations tab.";

            var messageBox = new CustomMaterialMessageBox
            {
                BtnOk = { Content = "Yes", Width = 150, Background = Brushes.MediumSeaGreen, Foreground = Brushes.Black, BorderBrush = Brushes.SeaGreen },
                BtnCancel = { Content = "No", Width = 150, Foreground = Brushes.Black, BorderBrush = Brushes.SeaGreen },

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

        private void OnGenerateReportRequested(object sender, EventArgs e)
        {
            var dialogViewModel = new GenerateReportDialogViewModel();
            var dialog = new GenerateReportDialog(dialogViewModel);
            dialog.ShowDialog();

            if (!dialogViewModel.Result)
            {
                _viewModel.GenerateReportResult = false;
                return;
            }

            _viewModel.GenerateReportResult = true;
            _viewModel.ReportStartDate = dialogViewModel.StartDate;
            _viewModel.ReportEndDate = dialogViewModel.EndDate;
        }

        private void OnOpenReportRequested(object sender, string filePath)
        {
            var dialog = new Report(filePath);
            dialog.ShowDialog();
        }
    }
}