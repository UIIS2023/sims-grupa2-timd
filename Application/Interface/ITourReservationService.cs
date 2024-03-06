using SimsProject.Domain.Model;
using System;
using System.Collections.Generic;

namespace SimsProject.Application.Interface
{
    public interface ITourReservationService : IService<TourReservation>
    {
        List<TourReservation> GetByGuide(User guide);
        void DeleteAllByParentId(int parentId); 
        List<TourReservation> GetByTour(Tour tour);
        List<TourReservation> GetByGuest(User guest);
        List<TourReservation> GetByGuestPresence(User guest);
        List<TourReservation> GetAll();
        TourReservation Save(TourReservation tourReservation);
        void Delete(TourReservation tourReservation);
        List<int> CountGuestsByAgeGroup(int tourId);
        List<Tour> GetReservedToursByUser(User user, List<Tour> allTours);
        List<TourReservation> GetByTourAndDate(int tourId, DateTime date);
        List<User> GetUsersByTourAndDate(int tourId, DateTime date);
        int CurrentTourOccupancy(Tour SlectedTour,string SelectedDate);
        int CountFreePlace(Tour tour, Tour SelectedTour, string SelectedDate);
        TourReservation CreateReservation(Tour SelectedTour, User LoggedInUser, string TBNumGuest, string SelectedDate, string TBAge,TourVoucher tourVoucher);
    }
}
