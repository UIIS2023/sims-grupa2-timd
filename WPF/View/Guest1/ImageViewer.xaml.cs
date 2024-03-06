using SimsProject.WPF.ViewModel.Guest1;
using System.Collections.ObjectModel;
using System.Windows;
using Image = SimsProject.Domain.Model.Image;

namespace SimsProject.WPF.View.Guest1
{
    public partial class ImageViewer : Window
    {
        public ImageViewer(ObservableCollection<Image> images, bool isDeletable)
        {
            InitializeComponent();
            DataContext = new ImageViewerViewModel(this, images, isDeletable);
        }
    }
}





