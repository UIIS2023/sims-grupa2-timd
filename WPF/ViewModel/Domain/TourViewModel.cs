using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using System.Collections.Generic;

namespace SimsProject.WPF.ViewModel.Domain
{
    public class TourViewModel : ViewModelBase
    {
        private readonly ITourService _tourService;
        private readonly ITourImageService _imageService;

        private readonly Tour _tour;

        public int Id => _tour.Id;
        public string Name => _tour.Name;
        public string Description => _tour.Description;
        public string Language => _tour.Language;
        public int MaxGuestNumber => _tour.MaxGuestNumber;
        public int Duration => _tour.Duration;
        public LocationViewModel TourLocation { get; }
        public ImageViewModel Cover { get; }
        public CheckPointListViewModel CheckPointsViewModel { get; }
        public TourDateListViewModel TourDatesViewModel { get; set; }
        public UserViewModel User { get; }
        public List<ImageViewModel> Images { get; }

        public TourViewModel(Tour tour)
        {
            _tourService = Injector.CreateInstance<ITourService>();
            _imageService = Injector.CreateInstance<ITourImageService>();

            _tour = _tourService.GetById(tour.Id);

            TourLocation = new LocationViewModel(_tour.TourLocation);
            Cover = new ImageViewModel(_imageService, _tour.Cover);
            CheckPointsViewModel = new CheckPointListViewModel(_tour);
            TourDatesViewModel = new TourDateListViewModel(_tour);
            User = new UserViewModel(_tour.Guide);

            Images = new List<ImageViewModel>();
            foreach (var image in _imageService.GetByParentId(_tour.Id))
            {
                Images.Add(new ImageViewModel(_imageService, image));
            }
        }
    }
}
