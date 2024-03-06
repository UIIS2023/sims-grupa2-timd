using SimsProject.Application.UseCase;
using SimsProject.Domain.Model;
using SimsProject.WPF.View.Guest2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace SimsProject.WPF.ViewModel.Guest2
{
    public class ComplexDetailsViewModel : ViewModelBase
    {
        public Page page;
     
        public User LoggedInUser;

        public ObservableCollection<TourRequest> Requests { get; set; }
        public static ComplexTourRequest SelectedRequest { get; set; }
        public TourRequestService tourRequestService;
        public LocationService locationService;
        public ComplexDetailsViewModel(User CurrentUser , Page _page, ComplexTourRequest selectedRequest) {
            page = _page;
          
            LoggedInUser= CurrentUser;

            tourRequestService = new();
            locationService = new();
            Requests = new(tourRequestService.GetRequestsByComplexId(selectedRequest));
            PopulateLocation();

        
        }

        

       
        public void PopulateLocation()
        {
            foreach (Location location in locationService.GetAll())
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

        public bool CanCancelClick() {
            return true;
        }

        public void CancelClick() {
            var navigationService = page.NavigationService;
            var form = new MyComplexRequests(LoggedInUser);
            navigationService.Navigate(form);
        }

    }
}
