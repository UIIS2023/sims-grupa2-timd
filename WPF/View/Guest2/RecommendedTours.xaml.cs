using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using SimsProject.Domain.Model;
using SimsProject.Repository;
using SimsProject.WPF.ViewModel.Guest2;

namespace SimsProject.WPF.View.Guest2
{
    /// <summary>
    /// Interaction logic for SearchTourWindow.xaml
    /// </summary>
    public partial class RecommendedTours : Window
    {
        /*
        public ObservableCollection<Tour> Tours { get; set; }
        public Tour SelectedTour { get; set; }
        public Location Location { get; set; }

        public Tour GivenTour { get; set; }
        public List<Location> Locations { get; set; }
        private readonly TourCsvRepository _tourRepository;
        private readonly LocationCsvRepository _locationRepository;
        private readonly CheckPointCsvRepository _checkPointRepository;
        private readonly TourDateCsvRepository _tourDateRepository;
        private readonly TourImageCsvRepository _imageRepository;
        private readonly UserCsvRepository _userRepository;

        public User CurrentUser { get; set; }
        */
        public RecommendedTours()
        {
            InitializeComponent();
           // this.DataContext = new RecommendedToursViewModel(currentUser,tour,this);
            /*
            CurrentUser = currentUser;
            GivenTour = tour;
            _tourRepository = new TourCsvRepository();
            _locationRepository = new LocationCsvRepository();
            _checkPointRepository = new CheckPointCsvRepository();
            _tourDateRepository = new TourDateCsvRepository();
            _userRepository = new UserCsvRepository();
            _imageRepository = new TourImageCsvRepository();
            Tours = new ObservableCollection<Tour>(_tourRepository.GetAll());

             PopulateTours();
             GetSortedCheckPoints();

            UpdateTable();

            */

        }

        /*
        private void PopulateTours()
        {
            foreach (var tour in Tours)
            {
               
                    tour.TourLocation = _locationRepository.GetById(tour.TourLocation.Id);
                    tour.CheckPoints = _checkPointRepository.GetByParentId(tour.Id);
                    tour.Guide = _userRepository.GetById(tour.Guide.Id);
                    tour.TourDates = _tourDateRepository.GetByParentId(tour.Id);
                    tour.Images = _imageRepository.GetByParentId(tour.Id);
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

        public bool isTourValid(Tour tour) {

            return (tour.Id!=GivenTour.Id && tour.TourLocation.Id==GivenTour.TourLocation.Id);

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
            foreach (var tour in _tourRepository.GetAll())
            {
                if (isTourValid(tour))
                {
                    Tours.Add(tour);
                    PopulateTours();
                    GetSortedCheckPoints();
                }
            }
        }




        private void ReserveClick(object sender, RoutedEventArgs e)
        {
            if (SelectedTour != null)
            {
                ReserveTourForm form = new(SelectedTour, CurrentUser);
                form.Show();
            }
            else
            {
                MessageBox.Show("Please select Tour.");
            }
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
        */
    }
}