using System;
using System.Collections.Generic;
using SimsProject.Domain.Model;

namespace SimsProject.Domain.RepositoryInterface
{
    public interface ITourReservationRepository : IRepository<TourReservation>
    {
        List<TourReservation> GetByTour(Tour tour);
        List<TourReservation> GetByGuide(User owner);
        void DeleteAllByParentId(int parentId);
        List<TourReservation> GetByGuest(User guest);
        List<TourReservation> GetByTourAndDate(int tourId, DateTime tourDate);
        List<User> GetUsersByTourAndDate(int tourId, DateTime tourDate);
        List<Tour> GetReservedToursByUser(User user, List<Tour> allTours);
    }
}
