
using SimsProject.Application.UseCase;
using SimsProject.Domain.Model;
using SimsProject.WPF.View.Guest2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace SimsProject.WPF.ViewModel.Guest2
{
    public class UnreviewedToursViewModel
    {
        private Action UpdateReviewsDelegate;
        public User LoggedInUser;
        public Page page;
        public static TourReservation SelectedTourReservation { get; set; }
        public static ObservableCollection<TourReservation> TourReservations { get; set; }


        public TourReservationService tourReservationService;
        public TourReviewService tourReviewService;
        public TourService tourService;
     


        public UnreviewedToursViewModel(User currentUser,Page p,Action updateReviewsDelegate)
        {

            LoggedInUser = currentUser;
            tourReservationService = new();
            tourReviewService = new();
            tourService = new();
            UpdateReviewsDelegate = updateReviewsDelegate;
            page = p;
            TourReservations = new(tourReservationService.GetByGuestPresence(LoggedInUser));
            PopulateTourName();
            UpdateTable();
           

        }

        private ICommand _reviewCommand;
        public ICommand ReviewCommand
        {
            get
            {
                if (_reviewCommand == null)
                {
                    _reviewCommand = new RelayCommand(
                        param => this.ReviewClick(),
                        param => this.CanReviewClick()
                    );
                }
                return _reviewCommand;
            }
        }
        public bool CanReviewClick()
        {
            return SelectedTourReservation != null;
        }

        private ICommand _backCommand;
        public ICommand BackCommand
        {
            get
            {
                if (_backCommand == null)
                {
                    _backCommand = new RelayCommand(
                        param => this.BackClick(),
                        param => this.CanBackClick()
                    );
                }
                return _backCommand;
            }
        }
        public bool CanBackClick()
        {
            return true;
        }

        public void RemoveGradedReservations() { 
            for (int i = TourReservations.Count - 1; i >= 0; i--)
            {
                if (tourReviewService.ExistsReview(TourReservations[i]) )
                {
                    TourReservations.RemoveAt(i);
                }
            }

        }

        private void UpdateTable()
        {
            TourReservations.Clear();
            foreach (var tourReservation in tourReservationService.GetByGuestPresence(LoggedInUser))
            {
                if (!tourReviewService.ExistsReview(tourReservation))
                {
                    TourReservations.Add(tourReservation);
                    PopulateTourName();
                }

            }
        }

        public void ReviewClick()
        {
            var navigationService = page.NavigationService;
            var form = new CreateReview(LoggedInUser, SelectedTourReservation,UpdateTable);
            navigationService.Navigate(form);

        }

        public void BackClick() {
            var navigationService = page.NavigationService;
           // var MyReviewsPage = new MyReviews(LoggedInUser);
            navigationService.GoBack();
            UpdateReviewsDelegate();
        }

        public void PopulateTourName() {
            for (int i = TourReservations.Count - 1; i >= 0; i--)
            {
                foreach (Tour tour in tourService.GetAll())
                {
                    if (TourReservations[i].Tour.Id == tour.Id)
                    {
                        TourReservations[i].Tour.Name = tour.Name;
                    }
                }
            }
            
        }
    }
}
