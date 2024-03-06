using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using SimsProject.Application.UseCase;
using SimsProject.Domain.Model;
using SimsProject.WPF.View.Guest2;

namespace SimsProject.WPF.ViewModel.Guest2
{
    public class MyRequestsViewModel : ViewModelBase
    {
        public User LoggedInUser;
        public Page _thisPage;
        public TourRequestService _tourRequestService;
        public UserService _userService;
        public LocationService _locationService;
        public ObservableCollection<TourRequest> Requests { get; set; }
        public List<User> Users;
        public List<Location> Locations;

      
    
        public MyRequestsViewModel(User CurrentUser, Page page) {

            LoggedInUser = CurrentUser;
            _thisPage = page;
            _tourRequestService = new TourRequestService();
            _userService = new UserService();
            _locationService = new LocationService();
            Users = _userService.GetAll();
            Locations = _locationService.GetAll();
            Requests = new(_tourRequestService.GetAll());
           
            UpdateRequests();
            
        }

        public void UpdateRequests() {

            Requests.Clear();
            foreach (var request in _tourRequestService.GetAll())
            {
                Requests.Add(request);
                _tourRequestService.CheckDateRange(request);
            }
            PopulateUsername();
            PopulateLocation();


        }
        public void PopulateUsername() {
            foreach (User user in Users)
            {
                for (int i = Requests.Count - 1; i >= 0; i--)
                {
                    if (Requests[i].Guide.Id == user.Id)
                    {
                        Requests[i].Guide.Username = user.Username;
                    }
                }
            }
        }
        public void PopulateLocation() {
            foreach (Location location in Locations)
            {
                for (int i = Requests.Count - 1; i >= 0; i--)
                {
                    if (Requests[i].Location.Id == location.Id)
                    {
                        Requests[i].Location.City = location.City;
                        Requests[i].Location.Country = location.Country;
                    }
                }
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
        public bool CanCreateClick()
        {
            return true;
        }

        public void CreateClick()
        {
            var form = new CreateRequest(LoggedInUser);
            var navigationService = _thisPage.NavigationService;
            navigationService.Navigate(form);


        }
        private ICommand _messageCommand;
        public ICommand MessageCommand
        {
            get
            {
                if (_messageCommand == null)
                {
                    _messageCommand = new RelayCommand(
                        param => this.MessageClick(),
                        param => this.CanViewMessageClick()
                    );
                }
                return _messageCommand;
            }
        }
        public bool CanViewMessageClick()
        {
            return true;
        }

        public void MessageClick() {
            var window = new Notifications(LoggedInUser);
            window.Show();
        }

        private ICommand _statisticsCommand;
        public ICommand StatisticsCommand
        {
            get
            {
                if (_statisticsCommand == null)
                {
                    _statisticsCommand = new RelayCommand(
                        param => this.StatisticsClick(),
                        param => this.CanViewStatisticsClick()
                    );
                }
                return _statisticsCommand;
            }
        }
        public bool CanViewStatisticsClick()
        {
            return true;
        }

        public void StatisticsClick(){
            var form = new Statistics(LoggedInUser);
            var navigationService = _thisPage.NavigationService;
            navigationService.Navigate(form);
        }

       
    }
}
