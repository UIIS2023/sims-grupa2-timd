using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.WPF.ViewModel.Domain;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Navigation;

namespace SimsProject.WPF.ViewModel.Guest1
{
    public class StatusOfMoveRequestsFormViewModel
    {
        public User CurrentUser;
        public NavigationService NavigationService { get; set; }
        public static ObservableCollection<AccommodationReservationRescheduleViewModel> Requests { get; set; }
        private IAccommodationReservationRescheduleService _accommodationReservationRescheduleService;
        public StatusOfMoveRequestsFormViewModel(User currentUser, NavigationService navigationService)
        {
            CurrentUser = currentUser;
            NavigationService = navigationService;
            _accommodationReservationRescheduleService = Injector.CreateInstance<IAccommodationReservationRescheduleService>();
            Requests = new ObservableCollection<AccommodationReservationRescheduleViewModel>();
            foreach (var request in _accommodationReservationRescheduleService.GetByGuest(currentUser))
            {
                Requests.Add(new AccommodationReservationRescheduleViewModel(request));
            }
            GoBackCommand = new RelayCommand(GoBackClick);
        }
        public ICommand GoBackCommand { get; set; }
        public void GoBackClick(object obj)
        {
            NavigationService.GoBack();
        }
    }
}
