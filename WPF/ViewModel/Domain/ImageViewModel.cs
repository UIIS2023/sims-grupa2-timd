using SimsProject.Application.Interface;
using SimsProject.Domain.Model;

namespace SimsProject.WPF.ViewModel.Domain
{
    public class ImageViewModel : ViewModelBase
    {
        private readonly Image _image;

        internal int Id => _image.Id;
        public string Url => _image.Url;

        public ImageViewModel(IImageService imageService, Image image)
        {
            _image = imageService.GetById(image.Id);
        }
    }
}
