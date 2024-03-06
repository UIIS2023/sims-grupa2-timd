using SimsProject.Application.UseCase;
using SimsProject.Domain.Model;
using SimsProject.WPF.View.Guest2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Image = SimsProject.Domain.Model.Image;

namespace SimsProject.WPF.ViewModel.Guest2
{
    public class MyReviewsViewModel
    {

        public static ObservableCollection<TourReview> Reviews { get; set; }
        public static List<Image> ImagesByParentId;
        public static List<Image> ReviewImages=new();
        public static TourReviewService tourReviewService;
        public static TourReviewImageService tourReviewImageService;
        public static TourService tourService;
        public User LoggedInUser;
        public NavigationService service;
        public MyReviewsViewModel(User currentUser,NavigationService _service) {
            LoggedInUser = currentUser;
            service=_service;
            tourReviewService = new();
            tourReviewImageService = new();
            ImagesByParentId = new();
            tourService = new();
            PopulateReviews();
            PopulateTourName();
        }
        public void PopulateReviews() {
            ReviewImages = tourReviewImageService.GetAll();
            Reviews = new(tourReviewService.GetByGuest(LoggedInUser));
            PopulateImagesByParentId();
        }
        public void PopulateImagesByParentId() {
            for (int i = Reviews.Count - 1; i >= 0; i--) {
                List<Image> imagesForReview = new List<Image>();
                foreach (Image reviewImage in ReviewImages) {
                    if (Reviews[i].Id == reviewImage.ParentId) {
                       imagesForReview.Add(reviewImage);
                    }
                }
                Reviews[i].Images= imagesForReview;
                
            }
            
        }

        public void PopulateTourName()
        {
            for (int i = Reviews.Count - 1; i >= 0; i--)
            {
                foreach (Tour tour in tourService.GetAll()) {
                    if (tour.Id == Reviews[i].Tour.Id)
                    {
                        Reviews[i].Tour.Name = tour.Name;
                    }
                }

            }
        }

        private ICommand _createReviewCommand;
        public ICommand CreateReviewCommand
        {
            get
            {
                if (_createReviewCommand == null)
                {
                    _createReviewCommand = new RelayCommand(
                        param => this.CreateReview_Click(),
                        param => this.CanCreateReviewClick()
                    );
                }
                return _createReviewCommand;
            }
        }
        public bool CanCreateReviewClick()
        {
            return true;
        }

        public void CreateReview_Click() {
            var form = new UnreviewedTours(LoggedInUser,PopulateReviews);
            service.Navigate(form);


        }

    }
}
