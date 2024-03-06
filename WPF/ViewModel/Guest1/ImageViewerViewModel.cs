using SimsProject.Domain.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace SimsProject.WPF.ViewModel.Guest1
{
    public class ImageViewerViewModel : ViewModelBase
    {
        private readonly Window _window; 
        public ObservableCollection<Image> Images { get; set; }
        public bool Deletable { get; set; }
        private int _index;
        public ImageViewerViewModel(Window window, ObservableCollection<Image> images, bool isDeletable)
        {
            _index = 0;
            _window = window;
            Images = images;
            Deletable = isDeletable;
            SelectedImage = Images[0];
            PreviousImageCommand = new RelayCommand(PreviousImageClick, CanPreviousImageClick);
            NextImageCommand = new RelayCommand(NextImageClick, CanNextImageClick);
            RemoveImageCommand = new RelayCommand(RemoveImageClick);
            GoBackCommand = new RelayCommand(GoBackClick);
        }
        public ICommand GoBackCommand { get; set; }
        public void GoBackClick(object obj)
        {
            _window.Close();
        }
        public ICommand PreviousImageCommand { get; set; }
        public bool CanPreviousImageClick(object obj)
        {
            return Images.Count > 1;
        }
        public void PreviousImageClick(object obj)
        {
            if (_index == 0)
            {
                _index = Images.Count - 1;
            }
            else
            {
                _index--;
            }
            SelectedImage = Images[_index];
        }
        public ICommand NextImageCommand { get; set; }
        public bool CanNextImageClick(object obj)
        {
            return Images.Count > 1;
        }
        public void NextImageClick(object obj)
        {
            _index = (_index + 1) % Images.Count;
            SelectedImage = Images[_index];
        }
        public ICommand RemoveImageCommand { get; set; }
        public void RemoveImageClick(object obj)
        {
            if (!Deletable) return;
            Images.Remove(SelectedImage);
            ReviewAccommodationViewModel._images.Remove(SelectedImage);
            if (Images.Count == 0)
            {
                _window.Close();
                return;
            }
            _index = (_index + 1) % Images.Count;
            SelectedImage = Images[_index];
        }
        private Image _selectedImage;
        public Image SelectedImage
        {
            get => _selectedImage;
            set
            {
                if (value == _selectedImage) return;
                _selectedImage = value;
                OnPropertyChanged(nameof(SelectedImage));
            }
        }
    }
}
