using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.WPF.ViewModel.Domain;

namespace SimsProject.WPF.ViewModel.Owner
{
    public class ReviewsPanelViewModel : ViewModelBase
    {
        public User Owner { get; set; }
        public ObservableCollection<AccommodationReviewViewModel> AccommodationReviews { get; set;  }
        public ObservableCollection<AccommodationReservationViewModel> ReviewableReservations { get; set;  }

        #region SERVICES

        private IReviewService _reviewService;
        private IGuestReviewService _guestReviewService;
        private IAccommodationReviewService _accommodationReviewService;
        private IAccommodationReservationService _accommodationReservationService;

        #endregion

        #region PROPERTIES

        private AccommodationReservationViewModel _selectedReservation;
        public AccommodationReservationViewModel SelectedReservation
        {
            get => _selectedReservation;
            set
            {
                if (value != _selectedReservation)
                {
                    _selectedReservation = value;
                    _isReservationSelected = _selectedReservation is not null;
                    OnPropertyChanged(nameof(SelectedReservation));
                    OnPropertyChanged(nameof(IsReservationSelected));
                }
            }
        }

        private bool _isReservationSelected;
        public bool IsReservationSelected
        {
            get => _isReservationSelected;
            set
            {
                if (value != _isReservationSelected)
                {
                    _isReservationSelected = value;
                    OnPropertyChanged(nameof(IsReservationSelected));
                }
            }
        }

        private int _cleanlinessGrade;
        public int CleanlinessGrade
        {
            get => _cleanlinessGrade;
            set
            {
                if (value != _cleanlinessGrade)
                {
                    _cleanlinessGrade = value;
                    OnPropertyChanged(nameof(CleanlinessGrade));
                }
            }
        }

        private int _observanceGrade;
        public int ObservanceGrade
        {
            get => _observanceGrade;
            set
            {
                if (value != _observanceGrade)
                {
                    _observanceGrade = value;
                    OnPropertyChanged(nameof(ObservanceGrade));
                }
            }
        }

        private string _comment;
        public string Comment
        {
            get => _comment;
            set
            {
                if (value != _comment)
                {
                    _comment = value;
                    OnPropertyChanged(nameof(Comment));
                }
            }
        }

        #endregion

        #region COMMANDS

        public ICommand ReviewCommand { get; set; }

        #endregion

        public ReviewsPanelViewModel(User owner)
        {
            Owner = owner;

            InitializeServices();
            InitializeCollections();
            InitializeCommands();
            SetDefaultValues();
        }

        private void InitializeServices()
        {
            _reviewService = Injector.CreateInstance<IReviewService>();
            _guestReviewService = Injector.CreateInstance<IGuestReviewService>();
            _accommodationReviewService = Injector.CreateInstance<IAccommodationReviewService>();
            _accommodationReservationService = Injector.CreateInstance<IAccommodationReservationService>();
        }

        private void InitializeCollections()
        {
            AccommodationReviews = new ObservableCollection<AccommodationReviewViewModel>();
            foreach (var review in _reviewService.GetGuestReviewedByOwner(Owner))
            {
                AccommodationReviews.Add(new AccommodationReviewViewModel(review));
            }

            ReviewableReservations = new ObservableCollection<AccommodationReservationViewModel>();
            foreach (var reservation in _reviewService.GetUnreviewedByOwner(Owner))
            {
                ReviewableReservations.Add(new AccommodationReservationViewModel(reservation));
            }
        }

        private void InitializeCommands()
        {
            ReviewCommand = new RelayCommand(Review, CanReview);
        }

        private void SetDefaultValues()
        {
            CleanlinessGrade = 1;
            ObservanceGrade = 1;
            Comment = null;
        }

        private void Review(object obj)
        {
            var commentWithoutNewline = Comment?.Replace(Environment.NewLine, "^") ?? "";
            var reservation = _accommodationReservationService.GetById(SelectedReservation.Id);

            var newGuestReview = new GuestReview(CleanlinessGrade, ObservanceGrade, commentWithoutNewline, Owner, SelectedReservation.Guest, reservation);
            var savedGuestReview = _guestReviewService.CreateReview(newGuestReview);

            RemoveGuestReview(savedGuestReview);
            ShowCorrespondingAccommodationReview(savedGuestReview);
            SetDefaultValues();
        }

        private void RemoveGuestReview(GuestReview savedGuestReview)
        {
            var reservationToRemove = ReviewableReservations.SingleOrDefault(r => r.Id == savedGuestReview.Reservation.Id);
            if (reservationToRemove is not null) ReviewableReservations.Remove(reservationToRemove);
        }

        private void ShowCorrespondingAccommodationReview(GuestReview savedGuestReview)
        {
            var accommodationReviewToShow = _accommodationReviewService.GetByReservation(savedGuestReview.Reservation);
            if (accommodationReviewToShow is not null)
            {
                AccommodationReviews.Add(new AccommodationReviewViewModel(accommodationReviewToShow));
            }
        }

        private bool CanReview(object obj)
        {
            return SelectedReservation is not null;
        }
    }
}
