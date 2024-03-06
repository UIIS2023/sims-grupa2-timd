using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.Domain.RepositoryInterface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimsProject.Application.UseCase
{
    public class TourReservationService : ITourReservationService
    {
        private readonly ITourReservationRepository _tourReservationRepository;
        private readonly ITourDateService _tourDateService;
        private readonly ITourAttendanceService _tourAttendanceService;
        private readonly ITourReviewService _tourReviewService;
        private readonly ITourVoucherService _tourVoucherService;
        
        public TourReservationService()
        {
            _tourReservationRepository = Injector.CreateInstance<ITourReservationRepository>();
            _tourDateService = Injector.CreateInstance<ITourDateService>();
            _tourAttendanceService = Injector.CreateInstance<ITourAttendanceService>();
            _tourReviewService = Injector.CreateInstance<ITourReviewService>();
            _tourVoucherService = Injector.CreateInstance<ITourVoucherService>();
        }

        public TourReservation GetById(int id)
        {
            return _tourReservationRepository.GetById(id);
        }

        public List<TourReservation> GetByGuide(User guide)
        {
            return _tourReservationRepository.GetByGuide(guide);
        }

        public void DeleteAllByParentId(int parentId)
        {
            _tourReservationRepository.DeleteAllByParentId(parentId);
        }

        public List<TourReservation> GetByTour(Tour tour)
        {
            return _tourReservationRepository.GetByTour(tour);
        }

        public List<TourReservation> GetByGuest(User guest) 
        {
            return _tourReservationRepository.GetByGuest(guest);
        }

        public List<int> CountGuestsByAgeGroup(int tourId)
        {
            var ageGroups = new List<int>(new int[4]);

            var guestsUnder18 = 0;
            var guestsBetween18And50 = 0;
            var guestsOver50 = 0;
            var totalGuests = 0;
            foreach (var reservation in _tourReservationRepository.GetAll())
            {
                foreach (var _ in _tourDateService.GetAll().Where(date => reservation.Tour.Id == tourId && date.Tour.Id == tourId && date.HasEnded && reservation.Date == date.Date))
                {
                    switch (reservation.GuestAge)
                    {
                        case < 18:
                            guestsUnder18++;
                            break;
                        case >= 18 and <= 50:
                            guestsBetween18And50++;
                            break;
                        default:
                            guestsOver50++;
                            break;
                    }
                }

                totalGuests++;
            }

            ageGroups[0] = guestsUnder18;
            ageGroups[1] = guestsBetween18And50;
            ageGroups[2] = guestsOver50;
            ageGroups[3] = totalGuests;

            return ageGroups;
        }

        public List<TourReservation> GetByGuestPresence(User guest)
        {
            List<TourReservation> reservationsPresent = new();

            foreach (var reservation in _tourReservationRepository.GetAll())
            {
                reservationsPresent.AddRange(from attendance in _tourAttendanceService.GetAll() where WasGuestPresent(attendance, reservation, guest) && !_tourReviewService.ExistsReview(reservation) select reservation);
            }

            reservationsPresent = reservationsPresent.Distinct().ToList();
            return reservationsPresent;
        }

        public bool WasGuestPresent(TourAttendance attendance, TourReservation reservation, User guest) 
        {
            var userValid = attendance.User.Id == guest.Id && reservation.Guest.Id == guest.Id;
            // unused?
            // var present = (int)(attendance.Present) == 2;
            var tourValid = reservation.Tour.Id == attendance.Tour.Id;
            var dateValid = reservation.Date.CompareTo(DateTime.Today) < 0;

            return userValid && tourValid && dateValid && IsTourFinished(reservation);
        }

        public bool IsTourFinished(TourReservation reservation) 
        {
            var dates = _tourDateService.GetAll();
            return dates.Any(date => date.Tour.Id == reservation.Tour.Id && reservation.Date == date.Date && date.HasEnded);
        }

        public List<TourReservation> GetAll() 
        {
            return _tourReservationRepository.GetAll();
        }

        public void Delete(TourReservation tourReservation)
        {
            _tourReservationRepository.Delete(tourReservation);
        }

        public List<Tour> GetReservedToursByUser(User user, List<Tour> allTours) 
        { 
            return _tourReservationRepository.GetReservedToursByUser(user, allTours);
        }

        public List<User> GetUsersByTourAndDate(int tourId, DateTime date)
        {
            return _tourReservationRepository.GetUsersByTourAndDate(tourId, date);
        }

        public List<TourReservation> GetByTourAndDate(int tourId, DateTime date)
        {
            return _tourReservationRepository.GetByTourAndDate(tourId, date);
        }

        public TourReservation Save(TourReservation tourReservation) 
        {
            return _tourReservationRepository.Save(tourReservation);
        }

        public int CurrentTourOccupancy(Tour selectedTour, string selectedDate)
        {
            return _tourReservationRepository.GetAll().Where(t => t.Tour.Id == selectedTour.Id && t.Date == DateTime.Parse(selectedDate)).Sum(t => t.GuestNumber);
        }

        public int CountFreePlace(Tour tour,Tour selectedTour,string selectedDate)
        {
            var result = tour.MaxGuestNumber - CurrentTourOccupancy(selectedTour,selectedDate);
            return result;
        }

        public TourReservation CreateReservation(Tour selectedTour, User loggedInUser, string tbNumGuest, string selectedDate, string tbAge, TourVoucher tourVoucher)
        {
            TourReservation newReservation;

            if (tourVoucher is not null)
            {
                newReservation = new TourReservation(selectedTour, selectedTour.Guide, loggedInUser, Convert.ToInt32(tbNumGuest), DateTime.Parse(selectedDate), Convert.ToInt32(tbAge),true);
                _tourVoucherService.Delete(tourVoucher);
            }
            else 
            { 
                newReservation = new TourReservation(selectedTour, selectedTour.Guide, loggedInUser, Convert.ToInt32(tbNumGuest), DateTime.Parse(selectedDate), Convert.ToInt32(tbAge), false);
            }

            _tourReservationRepository.Save(newReservation);
            return newReservation;
        }
    }
}
