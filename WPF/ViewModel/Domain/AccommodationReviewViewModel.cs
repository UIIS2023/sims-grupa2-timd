using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using System;

namespace SimsProject.WPF.ViewModel.Domain
{
    public class AccommodationReviewViewModel : ViewModelBase
    {
        private readonly AccommodationReview _review;

        internal int Id => _review.Id;
        public int CleanlinessGrade => _review.CleanlinessGrade;
        public int OwnerGrade => _review.OwnerGrade;
        public string Comment => _review.Comment.Replace("^", Environment.NewLine);
        public User Owner { get; }
        public User Guest { get; }
        public AccommodationReservationViewModel Reservation { get; }

        public AccommodationReviewViewModel(AccommodationReview review)
        {
            var reviewService = Injector.CreateInstance<IAccommodationReviewService>();
            var userService = Injector.CreateInstance<IUserService>();
            _review = reviewService.GetById(review.Id);

            Guest = userService.GetById(_review.Guest.Id);
            Owner = userService.GetById(_review.Owner.Id);
            Reservation = new AccommodationReservationViewModel(_review.Reservation);
        }
    }
}
