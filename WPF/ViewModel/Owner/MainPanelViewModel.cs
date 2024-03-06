using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.WPF.ViewModel.Domain;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Navigation;

namespace SimsProject.WPF.ViewModel.Owner
{
    public class MainPanelViewModel : ViewModelBase
    {
        public User Owner { get; set; }
        public NavigationService NavigationService { get; set; }
        public ObservableCollection<AccommodationReservationViewModel> OngoingReservations { get; set; }

        #region SERVICES

        private IAccommodationReservationService _reservationService;
        private IOwnerService _ownerService;

        #endregion
        
        #region PROPERTIES

        public string MostPopularLocation { get; set; }
        public string LeastPopularLocation { get; set; }
        public bool IsRecommendationAvailable { get; set; }

        #endregion

        #region COMMANDS

        public ICommand OpenRegistrationCommand { get; set; }
        public ICommand OpenAccommodationsCommand { get; set; }

        #endregion

        public MainPanelViewModel(User owner, NavigationService navigationService)
        {
            Owner = owner;
            NavigationService = navigationService;

            InitializeServices();
            InitializeCollections();
            InitializeRecommendations();
            InitializeCommands();
        }

        private void InitializeServices()
        {
            _reservationService = Injector.CreateInstance<IAccommodationReservationService>();
            _ownerService = Injector.CreateInstance<IOwnerService>();
        }

        private void InitializeCommands()
        {
            OpenRegistrationCommand = new RelayCommand(OpenRegistration);
            OpenAccommodationsCommand = new RelayCommand(OpenAccommodations);
        }

        private void InitializeCollections()
        {
            var ongoingReservations = (from reservation in _reservationService.GetOngoingByOwner(Owner)
                                       select new AccommodationReservationViewModel(reservation)).ToList();
            OngoingReservations = new ObservableCollection<AccommodationReservationViewModel>(ongoingReservations.OrderByDescending(r => r.CheckoutDate).ToList());
        }

        private void InitializeRecommendations()
        {
            var recommendations = _ownerService.GetRecommendations(Owner);

            if (recommendations is null)
            {
                IsRecommendationAvailable = false;
                MostPopularLocation = "No recommendations available";
                LeastPopularLocation = "No recommendations available";
            }
            else
            {
                IsRecommendationAvailable = true;
                MostPopularLocation = new LocationViewModel(recommendations.Open).ToString();
                LeastPopularLocation = new LocationViewModel(recommendations.Close).ToString();
            }
        }

        private void OpenRegistration(object obj)
        {
            NavigationService.Navigate(new Uri("../../WPF/View/Owner/RegistrationPanel.xaml", UriKind.Relative));
        }

        private void OpenAccommodations(object obj)
        {
            NavigationService.Navigate(new Uri("../../WPF/View/Owner/AccommodationsPanel.xaml", UriKind.Relative));
        }
    }
}