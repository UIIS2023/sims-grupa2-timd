using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using System;

namespace SimsProject.WPF.ViewModel.Domain
{
    public class GuestReviewViewModel
    {
        private readonly GuestReview _review;

        internal int Id => _review.Id;
        public int CleanlinessGrade => _review.CleanlinessGrade;
        public int ObservanceGrade => _review.ObservanceGrade;
        public string Comment => _review.Comment.Replace("^", Environment.NewLine);
        public User Guest { get; }
        public User Owner { get; }
        public AccommodationReservationViewModel Reservation { get; }

        public GuestReviewViewModel(GuestReview guestReview)
        {
            var guestReviewService = Injector.CreateInstance<IGuestReviewService>();
            var userService = Injector.CreateInstance<IUserService>();
            _review = guestReviewService.GetById(guestReview.Id);

            Guest = userService.GetById(_review.Guest.Id);
            Owner = userService.GetById(_review.Owner.Id);
            Reservation = new AccommodationReservationViewModel(_review.Reservation);
        }
    }
}
