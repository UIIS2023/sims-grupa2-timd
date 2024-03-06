using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using SimsProject.WPF.ViewModel.Domain;

namespace SimsProject.WPF.ViewModel
{
    public class ImageCarouselViewModel : ViewModelBase
    {
        public ObservableCollection<ImageViewModel> Images { get; }

        #region PROPERTIES

        private int _index;

        private ImageViewModel _selectedImage;
        public ImageViewModel SelectedImage
        {
            get => _selectedImage;
            set
            {
                if (value != _selectedImage)
                {
                    _selectedImage = value;
                    OnPropertyChanged(nameof(SelectedImage));
                }
            }
        }

        #endregion

        #region COMMANDS

        public ICommand NextImageCommand { get; set; }
        public ICommand PreviousImageCommand { get; set; }

        #endregion

        public ImageCarouselViewModel(List<ImageViewModel> images)
        {
            Images = new ObservableCollection<ImageViewModel>(images);
            SelectedImage = Images[0];
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            NextImageCommand = new RelayCommand(NextImage, CanNextImage);
            PreviousImageCommand = new RelayCommand(PreviousImage, CanPreviousImage);
        }

        public void NextImage(object obj)
        {
            SelectedImage = Images[++_index];
        }

        public bool CanNextImage(object obj)
        {
            return (_index + 1) < Images.Count;
        }

        public void PreviousImage(object obj)
        {
            SelectedImage = Images[--_index];
        }

        public bool CanPreviousImage(object obj)
        {
            return (_index - 1) >= 0;
        }
    }
}