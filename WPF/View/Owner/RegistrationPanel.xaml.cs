using System.Windows;
using SimsProject.WPF.ViewModel;
using SimsProject.WPF.ViewModel.Owner;
using System.Windows.Controls;
using System.Windows.Media;
using BespokeFusion;
using System;
using Microsoft.Win32;

namespace SimsProject.WPF.View.Owner
{
    /// <summary>
    /// Interaction logic for RegistrationPanel.xaml
    /// </summary>
    public partial class RegistrationPanel : Page
    {
        private readonly RegistrationPanelViewModel _viewModel;

        public RegistrationPanel()
        {
            InitializeComponent();

            var appViewModel = (AppViewModel)System.Windows.Application.Current.Resources["AppViewModel"];

            _viewModel = new RegistrationPanelViewModel(appViewModel.LoggedInUser, appViewModel.NavigationService);
            _viewModel.ConfirmActionRequested += OnConfirmActionRequested;
            _viewModel.UploadImagesFromComputerRequested += OnUploadImagesFromComputerRequested;

            DataContext = _viewModel;
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.Text)) return;
            
            var url = e.Data.GetData(DataFormats.Text) as string;
            try
            {
                _viewModel.AddImageFromWeb(url);
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid URL", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnUploadImagesFromComputerRequested(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files|*.jpeg;*.jpg;*.png",
                FilterIndex = 1,
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == false) return;

            try
            {
                _viewModel.AddImagesFromComputer(openFileDialog.FileNames);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Error adding image from the computer: {exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnConfirmActionRequested(object sender, EventArgs e)
        {
            var caption = "Discard accommodation";
            var text = "If you proceed, any unsaved changes you've made to this accommodation registration will be lost.\n" +
                       "Otherwise, click 'Cancel' to return to the registration form and save your changes";

            var messageBox = new CustomMaterialMessageBox
            {

                BtnOk = { Content = "Discard Changes", Width = 155, Background = Brushes.MediumSeaGreen, Foreground = Brushes.Black, BorderBrush = Brushes.SeaGreen },
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
