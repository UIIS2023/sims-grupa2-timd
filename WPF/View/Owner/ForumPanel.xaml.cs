using BespokeFusion;
using SimsProject.WPF.ViewModel.Owner;
using SimsProject.WPF.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SimsProject.WPF.View.Owner
{
    /// <summary>
    /// Interaction logic for ForumPanel.xaml
    /// </summary>
    public partial class ForumPanel : Page
    {
        private readonly ForumPanelViewModel _viewModel;

        public ForumPanel()
        {
            InitializeComponent();

            var appViewModel = (AppViewModel)System.Windows.Application.Current.Resources["AppViewModel"];

            _viewModel = new ForumPanelViewModel(appViewModel.LoggedInUser);
            _viewModel.ConfirmActionRequested += OnConfirmActionRequested;

            DataContext = _viewModel;
        }

        private void OnConfirmActionRequested(object sender, EventArgs e)
        {
            var caption = "Report comment";
            var text = "Are you sure you want to report this comment as false?\n" +
                       "The user who posted the comment will not be notified of you report.";

            var messageBox = new CustomMaterialMessageBox
            {
                BtnOk = { Content = "Report", Width = 150, Background = Brushes.MediumSeaGreen, Foreground = Brushes.Black, BorderBrush = Brushes.SeaGreen },
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
    }
}
