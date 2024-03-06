using System.Collections.Generic;
using System;
using System.Linq;
using SimsProject.Application.Interface;
using SimsProject.Domain.Model;

namespace SimsProject.Application.UseCase
{
    public class Guest1Service : IGuest1Service
    {
        private readonly IAccommodationReservationService _accommodationReservationService;
        private readonly IGuestReviewService _reviewService;
        private readonly IUserService _userService;
        public Guest1Service()
        {
            _accommodationReservationService = Injector.CreateInstance<IAccommodationReservationService>();
            _reviewService = Injector.CreateInstance<IGuestReviewService>();
            _userService = Injector.CreateInstance<IUserService>();
        }

        public void DeductPoint(User user)
        {
            if (user.SuperUserPoints == 0) return;
            user.SuperUserPoints--;
            _userService.Update(user);
        }

        private User SuperGuestObtained(User user)
        {
            user.SuperUserPoints = 5;
            user.IsSuperUser = true;
            user.BecameSuperUser = DateOnly.FromDateTime(DateTime.Today);
            _userService.Update(user);
            return user;
        }

        private User SuperGuestRevoked(User user)
        {
            user.SuperUserPoints = 0;
            user.IsSuperUser = false;
            _userService.Update(user);
            return user;
        }

        public User CheckSuperGuest(User user)
        {
            var reservations = _accommodationReservationService.GetByGuest(user);
            if (DateOnly.FromDateTime(DateTime.Today) < user.BecameSuperUser.AddYears(1)) return user;
            var i = 0;
            foreach (var reservation in reservations)
            {
                if (reservation.ArrivalDate >= DateOnly.FromDateTime(DateTime.Today.AddYears(-1)) &&
                    reservation.ArrivalDate < DateOnly.FromDateTime(DateTime.Today))
                {
                    i++;
                }
            }
            return (i >= 10) ? SuperGuestObtained(user) : SuperGuestRevoked(user);
        }
        public int GetNumberOfReservations(User currentUser)
        {
            return _accommodationReservationService.GetByGuest(currentUser).Count();
        }

        public int GetNumberOfReservationsPastYear(User currentUser)
        {
            return _accommodationReservationService.GetByGuest(currentUser).Count(x => x.ArrivalDate > DateOnly.FromDateTime(DateTime.Today).AddYears(-1) && x.ArrivalDate < DateOnly.FromDateTime(DateTime.Today));
        }

        public int GetNumberOfReservationsPastPeriod(User currentUser)
        {
            return _accommodationReservationService.GetByGuest(currentUser).Count(x => x.ArrivalDate > currentUser.BecameSuperUser && x.ArrivalDate < DateOnly.FromDateTime(DateTime.Today));
        }
        public double GetAverageCleanliness(User user)
        {
            List<GuestReview> list = new List<GuestReview>(_reviewService.GetByGuest(user));
            if (list.Count == 0) return 0.0;
            double average = 0;
            int count = 0;
            foreach (var x in list)
            {
                average += x.CleanlinessGrade;
                count++;
            }
            return average / count;
        }

        public double GetAverageObservance(User user)
        {
            List<GuestReview> list = new List<GuestReview>(_reviewService.GetByGuest(user));
            if (list.Count == 0) return 0.0;
            double average = 0;
            int count = 0;
            foreach (var x in list)
            {
                average += x.ObservanceGrade;
                count++;
            }
            return average / count;
        }
    }
}
