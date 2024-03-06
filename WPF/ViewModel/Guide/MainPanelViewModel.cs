using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.WPF.ViewModel.Domain;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SimsProject.WPF.ViewModel.Guide
{
    public class MainPanelViewModel : ViewModelBase
    {
        #region PROPERTIES

        public User Guide { get; set; }
        public static ObservableCollection<TourViewModel> Tours { get; set; }

        private TourViewModel _selectedTour;
        public TourViewModel SelectedTour
        {
            get => _selectedTour;
            set
            {
                _selectedTour = value;
                OnPropertyChanged(nameof(SelectedTour));
            }
        }

        private ImageCarouselViewModel _imageCarouselViewModel;
        public ImageCarouselViewModel ImageCarouselViewModel
        {
            get => _imageCarouselViewModel;
            set
            {
                if (value != _imageCarouselViewModel)
                {
                    _imageCarouselViewModel = value;
                    OnPropertyChanged(nameof(ImageCarouselViewModel));
                }
            }
        }

        public static Dictionary<TourViewModel, ImageCarouselViewModel> ImageCarouselViewModels { get; set; }

        #endregion

        #region SERVICES

        private ITourService _tourService;

        #endregion

        public MainPanelViewModel(User guide)
        {
            Guide = guide;

            InitializeServices();
            InitializeCollections();
            GetSortedCheckPoints();
        }

        private void InitializeServices()
        {
            _tourService = Injector.CreateInstance<ITourService>();
        }

        private void InitializeCollections()
        {
            Tours = new ObservableCollection<TourViewModel>();
            ImageCarouselViewModels = new Dictionary<TourViewModel, ImageCarouselViewModel>();

            foreach (var tour in _tourService.GetByGuide(Guide))
            {
                var tourViewModel = new TourViewModel(tour);
                Tours.Add(tourViewModel);

                ImageCarouselViewModel = new ImageCarouselViewModel(new TourViewModel(tour).Images.ToList());
                ImageCarouselViewModels[tourViewModel] = ImageCarouselViewModel;
            }
        }

        public static void GetSortedCheckPoints()
        {
            foreach (var tour in Tours)
            {
                SortCheckPoints(tour.CheckPointsViewModel.CheckPoints);
            }
        }

        public static void SortCheckPoints(ObservableCollection<CheckPointViewModel> checkPoints)
        {
            var lastCheckPoint = checkPoints[1];
            checkPoints.RemoveAt(1);
            checkPoints.Add(lastCheckPoint);
        }
    }
}