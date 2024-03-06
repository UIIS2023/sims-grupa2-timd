using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using SimsProject.Domain.Model;
using SimsProject.Repository;
using SimsProject.WPF.ViewModel.Guest2;

namespace SimsProject.WPF.View.Guest2
{
    /// <summary>
    /// Interaction logic for SearchTourWindow.xaml
    /// </summary>
    public partial class ExploreTours : Window
    {
        /*
        public ObservableCollection<Tour> Tours { get; set; }
        public Tour SelectedTour { get; set; }
        public Location Location { get; set; }
        public List<Location> Locations { get; set; }
        private readonly TourCsvRepository _tourRepository;
        public TourReservationCsvRepository tourReservationRepository;
        private readonly LocationCsvRepository _locationRepository;
        private readonly CheckPointCsvRepository _checkPointRepository;
        private readonly TourDateCsvRepository _tourDateRepository;
        private readonly TourImageCsvRepository _imageRepository;
        private readonly UserCsvRepository _userRepository;
        public ObservableCollection<TourReservation> tourReservations;

        public User CurrentUser { get; set; }
        */
        public ExploreTours()
        {
            InitializeComponent();
           // this.DataContext = new ExploreViewModel(currentUser);
            /*
            CurrentUser = currentUser;

            _tourRepository = new TourCsvRepository();
            _locationRepository = new LocationCsvRepository();
            _checkPointRepository = new CheckPointCsvRepository();
            _tourDateRepository = new TourDateCsvRepository();
            _userRepository = new UserCsvRepository();
            tourReservationRepository = new TourReservationCsvRepository();
            _imageRepository = new TourImageCsvRepository();
            Tours = new ObservableCollection<Tour>(_tourRepository.GetAll());
            tourReservations = new(tourReservationRepository.GetAll());

            Tours = new ObservableCollection<Tour>(_tourRepository.GetAll());
            ObservableCollection<string> Countries = new(_locationRepository.GetAllCountries());
            ObservableCollection<string> Cities = new(_locationRepository.GetAllCities());
           
            PopulateTours();
            GetSortedCheckPoints();
            //UpdateTable();

            foreach (String country in Countries)
            {
                ComboBoxCountry.Items.Add(country);
            }
            foreach (String city in Cities)
            {
               
                    ComboBoxCity.Items.Add(city);

            }
           
            */
        }

        /*
        private void PopulateTours()
        {
            foreach (var tour in Tours)
            {
                tour.TourLocation = _locationRepository.GetById(tour.TourLocation.Id);
                tour.CheckPoints = _checkPointRepository.GetByParentId(tour.Id);
                tour.User = _userRepository.GetById(tour.User.Id);
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
      
        private bool isTourValid(Tour tour)
        {
            return (isLocationValid(tour) && isLanguageValid(tour) && isGuestNumValid(tour) && isTourDurationValid(tour));
        }

        public bool isLocationValid( Tour tour) {

            if ( _locationRepository.GetById(tour.TourLocation.Id).Country == ComboBoxCountry.SelectedItem.ToString())
            {
                if ( _locationRepository.GetById(tour.TourLocation.Id).City == ComboBoxCity.SelectedItem.ToString() )
                {
                    return true;
                }
            }
            return false;
        }

        public bool isGuestNumValid(Tour tour) {
            if ( tour.MaxGuestNumber >= Convert.ToInt32(TextBoxNumGuest.Text) )
            {
                return true;
            }
            return false;
        }

        public bool  isTourDurationValid(Tour tour) {

            if (TextBoxDuration.Text == "" || tour.Duration == Convert.ToInt32(TextBoxDuration.Text))
            {
                return true;
            }
            return false;
        }

        public bool isLanguageValid(Tour tour) {

            if ( tour.Language == "" || tour.Language.ToLower().Contains(TextBoxLanguage.Text.ToLower()))
            {
                return true;
            }
            return false;

        }

        private void SearchClick(object sender, RoutedEventArgs e)
        {

            if (isSelectionValid())
            {
                UpdateTable();
            }
        }
 
        public bool isSelectionValid()
        {
            if (ComboBoxCountry.SelectedIndex == -1)
            {
                ComboBoxCountry.Focus();
                MessageBox.Show("Select country!");
                return false;
            }
            else
            {
                if (ComboBoxCity.SelectedIndex == -1)
                {
                    ComboBoxCity.Focus();
                    MessageBox.Show("Select city!");
                    return false;
                }
                else
                {
                    if (TextBoxNumGuest.Text == "")
                    {
                        MessageBox.Show("Choose Guest Number!");
                        return false;
                    }
                    else
                    {
                        return true;
                    }
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

        private void CountrySelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ComboBoxCity.Items.Clear();
            ObservableCollection<Location> locations = new(_locationRepository.GetAllLocationsByCountry(ComboBoxCountry.SelectedItem.ToString()));
            foreach(Location location in locations) { 
                ComboBoxCity.Items.Add ( location.City);
            }
        }

        */

    }
}