using SimsProject.Application.UseCase;
using SimsProject.Domain.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace SimsProject.WPF.ViewModel.Guest2
{
    public class MyVouchersViewModel : INotifyPropertyChanged
    {

        public TourVoucherService tourVoucherService;
        public TourReservationService tourReservationService;
        public event PropertyChangedEventHandler PropertyChanged;
        public static ObservableCollection<TourVoucher> Vouchers { get; set; }
        public List<TourReservation> Reservations;
        public User LoggedInUser;

        public ImageSource Image { get; set; }
        public static ImageSource SharedImage { get; } = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Gift.png"));
        public MyVouchersViewModel(User currentUser) {
            LoggedInUser = currentUser;
            tourVoucherService = new();
            tourReservationService = new();
            Vouchers = new(tourVoucherService.GetByGuest(LoggedInUser));
            Reservations= tourReservationService.GetAll();
           // tourVoucherService.DeservesVoucher(LoggedInUser);
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
    }
}
