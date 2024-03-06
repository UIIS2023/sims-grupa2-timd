using SimsProject.Application.UseCase;
using SimsProject.Domain.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimsProject.WPF.ViewModel.Guest2
{
    public class MyReservationsViewModel : ViewModelBase
    {
        public User LoggedInUser;
        public ObservableCollection<TourReservation> Reservations { get; set; }
        public TourReservationService tourReservationService;
        public TourService tourService;
        public LocationService locationService;
        public TourImageService imageService;
        public MyReservationsViewModel(User CurrentUser) { 
            LoggedInUser=CurrentUser;
            tourReservationService = new();
            tourService = new();
            locationService = new();
            imageService = new();
            Reservations = new(tourReservationService.GetByGuest(LoggedInUser));
            PopulateTours();


        }

        public void PopulateTours() {
            foreach (Tour tour in tourService.GetAll()) {
                foreach (TourReservation reservation in Reservations) {
                    if (tour.Id==reservation.Tour.Id) {
                        reservation.Tour.Name = tour.Name;
                        reservation.Tour.Language = tour.Language;
                        reservation.Tour.TourLocation = locationService.GetById(tour.TourLocation.Id);
                        reservation.Tour.Images = imageService.GetByParentId(tour.Id);
                        reservation.Tour.Cover = reservation.Tour.Images[0];
                    }
                }
            }
            
        }
    }
}
