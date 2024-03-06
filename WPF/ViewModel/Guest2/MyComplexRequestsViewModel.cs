using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using SimsProject.Application.UseCase;
using SimsProject.Domain.Model;
using SimsProject.WPF.View.Guest2;

namespace SimsProject.WPF.ViewModel.Guest2
{
    public class MyComplexRequestsViewModel : ViewModelBase
    {
        public User LoggedInUser;
        public List<User> Users;
        public List<Location> Locations;
        public UserService _userService;
        public LocationService _locationService;
        public NavigationService service;

        public ObservableCollection<ComplexTourRequest> ComplexRequests { get; set; }
        public ComplexTourRequestService complexTourRequestService;
        public TourRequestService tourRequestService;
        public MyComplexRequestsViewModel(User CurrentUser, NavigationService _service) {
            service = _service; ;
            LoggedInUser = CurrentUser;
            complexTourRequestService = new();
            tourRequestService = new();
            _userService = new();
            _locationService = new();
            Locations = new(_locationService.GetAll());
            Users = new(_userService.GetAll());
            ComplexRequests = new(complexTourRequestService.GetAll());
            PopulateRequests(ComplexRequests);
            PopulateLocation();
            PopulateUsername();
            CheckDateRange();
            CheckRequestsStatus();
        }
        private ComplexTourRequest _selectedRequest;
        public ComplexTourRequest SelectedRequest
        {
            get { return _selectedRequest; }
            set
            {
                _selectedRequest = value;
                OnPropertyChanged(nameof(SelectedRequest));
                DetailsClick();
            }
        }

        private ICommand _createCommand;
        public ICommand CreateCommand
        {
            get
            {
                if (_createCommand == null)
                {
                    _createCommand = new RelayCommand(
                        param => this.CreateClick(),
                        param => this.CanCreateClick()
                    );
                }
                return _createCommand;
            }
        }



        public void DetailsClick() {
           
            var form = new ComplexDetails(LoggedInUser, SelectedRequest);
            service.Navigate(form);
        }
        public void CreateClick() {
            var form = new CreateComplexRequest(LoggedInUser, new ObservableCollection<TourRequest>());
            service.Navigate(form);
        }
        public bool CanCreateClick() {
            return true;
        }

        public void PopulateRequests(ObservableCollection<ComplexTourRequest> complexRequests) {
            foreach (ComplexTourRequest complex in complexRequests) {
                complex.TourRequests = tourRequestService.GetRequestsByComplexId(complex);
            }
        }

        public void PopulateUsername()
        {
            foreach (User user in Users)
            {
                foreach (ComplexTourRequest complex in ComplexRequests) {
                    for (int i = complex.TourRequests.Count - 1; i >= 0; i--)
                    {
                        if (complex.TourRequests[i].Guide.Id == user.Id)
                        {
                            complex.TourRequests[i].Guide.Username = user.Username;
                        }
                    }
                }
            }
        }
        public void PopulateLocation()
        {
            foreach (Location location in Locations)
            {
                foreach (ComplexTourRequest complex in ComplexRequests) {
                    for (int i = complex.TourRequests.Count - 1; i >= 0; i--)
                    {
                        if (complex.TourRequests[i].Location.Id == location.Id)
                        {
                            complex.TourRequests[i].Location.City = location.City;
                            complex.TourRequests[i].Location.Country = location.Country;
                        }
                    }
                }
            }
        }

        public void CheckDateRange() {
            foreach (ComplexTourRequest complex in ComplexRequests) {
                tourRequestService.CheckDateRange(complex.TourRequests[0]);
                if ((int)(complex.TourRequests[0].RequestStatus)==1) {
                    complex.Status = ComplexRequestStatus.Invalid;
                }
            }
        }

        public void CheckRequestsStatus() {
            foreach (ComplexTourRequest complex in ComplexRequests)
            {
                if (tourRequestService.IsAllAccepted(complex.TourRequests)) {
                    complex.Status = ComplexRequestStatus.Accepted;
                    complexTourRequestService.Update(complex);
                }
            }
        }
    }
}
