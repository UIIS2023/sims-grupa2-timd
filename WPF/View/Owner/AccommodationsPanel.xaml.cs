using System.Windows;
using SimsProject.WPF.ViewModel;
using SimsProject.WPF.ViewModel.Owner;
using System.Windows.Controls;
using System.Windows.Media;
using BespokeFusion;
using System;

namespace SimsProject.WPF.View.Owner
{
    /// <summary>
    /// Interaction logic for AccommodationsPanel.xaml
    /// </summary>
    public partial class AccommodationsPanel : Page
    {
        private readonly AccommodationsPanelViewModel _viewModel;
        public YearlyStatisticsGrid YearlyStatistics { get; set; }
        public MonthlyStatisticsGrid MonthlyStatistics { get; set; }

        public AccommodationsPanel()
        {
            InitializeComponent();

            var appViewModel = (AppViewModel)System.Windows.Application.Current.Resources["AppViewModel"];

            _viewModel = new AccommodationsPanelViewModel(appViewModel.LoggedInUser, appViewModel.NavigationService);
            _viewModel.ConfirmActionRequested += OnConfirmActionRequested;

            DataContext = _viewModel;

            YearlyStatistics = new YearlyStatisticsGrid
            {
                DataContext = _viewModel
            };

            MonthlyStatistics = new MonthlyStatisticsGrid
            {
                DataContext = _viewModel
            };
        }

        private void OnConfirmActionRequested(object sender, EventArgs e)
        {
            var caption = "Delete accommodation";
            var text = $"Are you sure you want to close {_viewModel.SelectedAccommodation.Name}?\n" +
                       "Deleting this accommodation will permanently remove it from your account.\n" +
                       "Don't worry, if you make a mistake, you can always contact us for help!";

            var messageBox = new CustomMaterialMessageBox
            {
                BtnOk = { Content = "Delete", Width = 150, Background = Brushes.MediumSeaGreen, Foreground = Brushes.Black, BorderBrush = Brushes.SeaGreen },
                BtnCancel = { Content = "Cancel", Width = 150, Foreground = Brushes.Black, BorderBrush = Brushes.SeaGreen },

                Width = 400,
                Height =250,
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
