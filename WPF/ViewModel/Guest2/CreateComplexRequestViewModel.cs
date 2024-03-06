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
    public class CreateComplexRequestViewModel : ViewModelBase
    {
       public Page _page;
       public  User LoggedInUser;
       public TourRequestService _tourRequestService;
        public ComplexTourRequestService _complexTourRequestService;
      
        public  ObservableCollection<TourRequest> Requests { get; set; }
      

        public CreateComplexRequestViewModel(User CurrentUser,Page page,ObservableCollection<TourRequest> requests){
            _page = page;
            LoggedInUser = CurrentUser;
            _tourRequestService = new TourRequestService();
            _complexTourRequestService = new();

            Requests = new(requests);
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

    public void CreateClick() {

            //sacuvaj kompleksni zahtev
            //naviguj na listu svih zahtjeva
           
            _complexTourRequestService.CreateComplexRequest(Requests.ToList(),0);
            Navigate();
        
    }
        private ICommand _addCommand;
        public ICommand AddCommand
        {
            get
            {
                if (_addCommand == null)
                {
                    _addCommand = new RelayCommand(
                        param => this.AddClick(),
                        param => this.CanAddClick()
                    );
                }
                return _addCommand;
            }
        }
        public bool CanAddClick()
        {
            return true;
        }

        public void AddClick() {
              var form = new PartialRequest(LoggedInUser,Requests);
              var navigationService = _page.NavigationService;
              navigationService.Navigate(form);

        }
        public void Navigate() { 
            var navigationService= _page.NavigationService;
            var form = new MyComplexRequests(LoggedInUser);
            navigationService.Navigate(form);
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

        public void CancelClick() {
            Navigate();
        }
    }
}
