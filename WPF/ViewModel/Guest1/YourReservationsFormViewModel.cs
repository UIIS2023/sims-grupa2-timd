using SimsProject.Domain.Model;
using SimsProject.WPF.View.Guest1;
using SimsProject.WPF.ViewModel.Domain;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using SimsProject.Application.Interface;
using SimsProject.Application.Localization;
using System.Windows.Navigation;

namespace SimsProject.WPF.ViewModel.Guest1
{
    public class YourReservationsFormViewModel : ViewModelBase
    {
        public User CurrentUser;
        public NavigationService NavigationService { get; set; }
        public static AccommodationReservationViewModel SelectedReservation { get; set; }
        public static ObservableCollection<AccommodationReservationViewModel> Reservations { get; set; }
        private readonly IAccommodationReservationService _accommodationReservationService;
        public YourReservationsFormViewModel(User currentUser, NavigationService navigationService)
        {
            CurrentUser = currentUser;
            NavigationService = navigationService;
            SelectedReservation = null;
            _accommodationReservationService = Injector.CreateInstance<IAccommodationReservationService>();
            Reservations = new ObservableCollection<AccommodationReservationViewModel>();
            foreach (var reservations in _accommodationReservationService.GetUpcomingByGuest(currentUser))
            {
                Reservations.Add(new AccommodationReservationViewModel(reservations));
            }
            CancelReservationCommand = new RelayCommand(CancelReservationClick, CanCancelReservation);
            RequestMoveCommand = new RelayCommand(RequestMoveClick, CanRequestMoveClick);
            ViewStatusCommand = new RelayCommand(ViewStatusClick);
            PDFCommand = new RelayCommand(PDFClick);
        }
        public ICommand CancelReservationCommand { get; set; }
        private bool CanCancelReservation(object obj)
        {
            return SelectedReservation != null;
        }
        private void CancelReservationClick(object obj)
        {
            if (DateOnly.FromDateTime(DateTime.Today) < SelectedReservation.ArrivalDate.AddDays(-SelectedReservation.Accommodation.MinDaysBeforeCancellation))
            {
                _accommodationReservationService.CancelReservation(SelectedReservation.Id);
                Reservations.Remove(SelectedReservation);
                MessageBox.Show(TranslationSource.Instance["Canceled"]);
            }
            else
            {
                MessageBox.Show(TranslationSource.Instance["LateCancel"]);
            }
        }

        public ICommand RequestMoveCommand { get; set; }
        private bool CanRequestMoveClick(object obj)
        {
            return SelectedReservation != null;
        }
        private void RequestMoveClick(object obj)
        {
            if (DateOnly.FromDateTime(DateTime.Today) < SelectedReservation.ArrivalDate.AddDays(-SelectedReservation.Accommodation.MinDaysBeforeCancellation))
            {
                RequestReservationMoveForm form = new(SelectedReservation, CurrentUser);
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show(TranslationSource.Instance["LateReschedule"]);
            }
        }
        public ICommand ViewStatusCommand { get; set; }
        private void ViewStatusClick(object obj)
        {
            NavigationService.Navigate(
                new Uri("/SimsProject;component/WPF/View/Guest1/StatusOfMoveRequestsForm.xaml", UriKind.Relative));
        }
        public ICommand PDFCommand { get; set; }
        private void PDFClick(object obj)
        {
            GenerateWindow form = new(CurrentUser);
            form.ShowDialog();
        }
    }
}
