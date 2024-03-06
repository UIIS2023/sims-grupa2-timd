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

namespace SimsProject.WPF.ViewModel.Guest2
{
    public class RecommendedToursViewModel
    {

        public ObservableCollection<Tour> Tours { get; set; }
        public Tour SelectedTour { get; set; }
        public Location Location { get; set; }
        public Page window;
        public Tour GivenTour { get; set; }
        public List<Location> Locations { get; set; }
        private readonly TourService _tourService;
        private readonly LocationService _locationService;
        private readonly CheckPointService _checkPointService;
        private readonly TourDateService _tourDateService;
        private readonly TourImageService _imageService;
        private readonly UserService _userService;

        public User CurrentUser { get; set; }

        private ICommand _reserveCommand;
        public ICommand ReserveCommand
        {
            get
            {
                if (_reserveCommand == null)
                {
                    _reserveCommand = new RelayCommand(
                        param => this.ReserveClick(),
                        param => this.CanReserveClick()
                    );
                }
                return _reserveCommand;
            }
        }
        public bool CanReserveClick()
        {

            return true;
        }

        private ICommand _cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                {
                    _cancelCommand = new RelayCommand(
                        param => this.CancelClick(),
                        param => this.CanCancelClick()
                    );
                }
                return _cancelCommand;
            }
        }
        public bool CanCancelClick()
        {
            return true;
        }

        public RecommendedToursViewModel(User currentUser,Tour tour,Page w) {
            CurrentUser = currentUser;
            GivenTour = tour;
            _tourService = new ();
            _locationService = new ();
            _checkPointService = new ();
            _tourDateService = new ();
            _userService = new ();
            _imageService = new ();
            Tours = new ObservableCollection<Tour>(_tourService.GetAll());
            window = w;
            PopulateTours();
            GetSortedCheckPoints();

            UpdateTable();

        }

        private void PopulateTours()
        {
            foreach (var tour in Tours)
            {

                tour.TourLocation = _locationService.GetById(tour.TourLocation.Id);
                tour.CheckPoints = _checkPointService.GetByParentId(tour.Id);
                tour.Guide = _userService.GetById(tour.Guide.Id);
                tour.TourDates = _tourDateService.GetByParentId(tour.Id);
                tour.Images = _imageService.GetByParentId(tour.Id);
                tour.Cover = tour.Images[0];

            }
        }

        public void GetSortedCheckPoints()
        {
            foreach (var tour in Tours)
            {
                SortCheckPoints(tour.CheckPoints);
            }
        }

        public bool isTourValid(Tour tour)
        {

            return (tour.Id != GivenTour.Id && tour.TourLocation.Id == GivenTour.TourLocation.Id);

        }

        public void SortCheckPoints(List<CheckPoint> checkPoints)
        {
            CheckPoint lastCheckPoint = checkPoints[1];
            checkPoints.RemoveAt(1);
            checkPoints.Add(lastCheckPoint);
        }



        private void UpdateTable()
        {
            Tours.Clear();
            foreach (var tour in _tourService.GetAll())
            {
                if (isTourValid(tour))
                {
                    Tours.Add(tour);
                    PopulateTours();
                    GetSortedCheckPoints();
                }
            }
        }




        private void ReserveClick()
        {
            if (SelectedTour != null)
            {
               // ReserveTourForm form = new(SelectedTour, CurrentUser);
               // form.Show();
            }
            else
            {
                MessageBox.Show("Please select Tour.");
            }
        }

        private void CancelClick()
        {
          //  window.Close();

        }
    }
}
