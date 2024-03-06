using System.Windows;
using SimsProject.Domain.Model;
using SimsProject.WPF.ViewModel.Domain;
using SimsProject.WPF.ViewModel.Guest1;

namespace SimsProject.WPF.View.Guest1
{
    public partial class RequestReservationMoveForm : Window
    {
        public RequestReservationMoveForm(AccommodationReservationViewModel selectedReservation, User currentUser)
        {
            InitializeComponent();
            DataContext = new RequestReservationMoveViewModel(this, selectedReservation, currentUser);
        }
    }
}
