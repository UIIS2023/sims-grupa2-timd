using System.Collections.Generic;
using SimsProject.Application.Interface;
using SimsProject.Domain.Model;

namespace SimsProject.WPF.ViewModel.Domain
{
    public class TourReviewViewModel : ViewModelBase
    {
        private readonly ITourReviewService _tourReviewService;
        private readonly IImageService _imageService;

        private readonly TourReview _tourReview;

        public int Id => _tourReview.Id;
        public int GuideKnowledge => _tourReview.GuideKnowledge;
        public int GuideLanguage => _tourReview.GuideLanguage;
        public int TourInterestigness => _tourReview.TourInterestigness;
        public string Comment => _tourReview.Comment;
        public TourReservationViewModel Reservation { get; set; }
        public UserViewModel Guest { get; set; }
        public TourViewModel Tour { get; set; }
        public TourDateViewModel TourDate { get; set; }
        public ImageViewModel Cover { get; set; }
        public CheckPointViewModel CheckPoint { get; set; }
        public bool IsValid
        {
            get => _tourReview.IsValid;
            set => _tourReview.IsValid = value;
        }

        public TourReviewViewModel(TourReview tourReview)
        {
            _tourReviewService = Injector.CreateInstance<ITourReviewService>();
            _imageService = Injector.CreateInstance<ITourReviewImageService>();
            
            _tourReview = _tourReviewService.GetById(tourReview.Id);

            Reservation = new TourReservationViewModel(_tourReview.Reservation);
            TourDate = new TourDateViewModel(_tourReview.TourDate);
            Cover = new ImageViewModel(_imageService, _tourReview.Cover);
            Tour = new TourViewModel(_tourReview.Tour);
            Guest = new UserViewModel(_tourReview.Guest);
            CheckPoint = new CheckPointViewModel(_tourReview.CheckPoint);
        }
    }
}
