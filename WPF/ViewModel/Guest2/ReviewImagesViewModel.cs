using SimsProject.Domain.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimsProject.WPF.ViewModel.Guest2
{
    public class ReviewImagesViewModel : ViewModelBase
    {
        private ICommand _removeCommand;
        public ICommand RemoveCommand
        {
            get
            {
                if (_removeCommand == null)
                {
                    _removeCommand = new RelayCommand(
                        param => this.RemoveClick(),
                        param => this.CanRemoveClick()
                    );
                }
                return _removeCommand;
            }
        }
        public bool CanRemoveClick()
        {

            return SelectedImage!=null;
        }
        public ObservableCollection<Image> Images { get; set; }
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

        public ReviewImagesViewModel(ObservableCollection<Image> images) {
            Images = new(images);
        }

        public void RemoveClick() {
            CreateReviewViewModel.Images.Remove(SelectedImage);
            Images.Remove(SelectedImage);


        }
    }
}
