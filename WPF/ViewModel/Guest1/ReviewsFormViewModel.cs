using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.WPF.ViewModel.Domain;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Navigation;

namespace SimsProject.WPF.ViewModel.Guest1
{
    public class ReviewsFormViewModel
    {
        public User CurrentUser;
        public NavigationService NavigationService { get; set; }
        public DateOnly CurrentDate;
        public static AccommodationReservationViewModel SelectedAccommodationReservation { get; set; }
        public static ObservableCollection<AccommodationReservationViewModel> AccommodationReservations { get; set; }
        public static ObservableCollection<GuestReviewViewModel> GuestReviews { get; set; } 

        private readonly IReviewService _reviewService;

        public ReviewsFormViewModel(User currentUser, NavigationService navigationService)
        {
            CurrentUser = currentUser;
            NavigationService = navigationService;
            CurrentDate = DateOnly.FromDateTime(DateTime.Today);
            SelectedAccommodationReservation = null;
            _reviewService = Injector.CreateInstance<IReviewService>();

            AccommodationReservations = new ObservableCollection<AccommodationReservationViewModel>();
            foreach (var reservation in _reviewService.GetUnreviewedByGuest(currentUser))
            {
                AccommodationReservations.Add(new AccommodationReservationViewModel(reservation));
            }
            GuestReviews = new ObservableCollection<GuestReviewViewModel>();
            foreach (var review in _reviewService.GetReviewsByGuest(currentUser))
            {
                GuestReviews.Add(new GuestReviewViewModel(review));
            }
            ReviewCommand = new RelayCommand(ReviewClick, CanReviewClick);
        }
        public ICommand ReviewCommand { get; set; }
        public bool CanReviewClick(object obj)
        {
            return SelectedAccommodationReservation != null;
        }
        public void ReviewClick(object obj)
        {
            NavigationService.Navigate(new Uri("/SimsProject;component/WPF/View/Guest1/ReviewAccommodationForm.xaml", UriKind.Relative));
        }
    }
}
