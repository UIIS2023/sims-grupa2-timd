using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimsProject.Domain.Model;
using System.Xml.Linq;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using SimsProject.Application.UseCase;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using SimsProject.WPF.View.Guest2;
using SimsProject.WPF.View.Authentication;

namespace SimsProject.WPF.ViewModel.Guest2
{
    public class Guest2OverviewViewModel
    {
        public Window window;
        public static ObservableCollection<Tour> Tours { get; set; }
        public List<Tour> AllTours { get; set; }

        public List<TourReservation> TourReservations { get; set; }
        public List<TourAttendance> TourAttendances { get; set; }
        public Tour SelectedTour { get; set; }
        public User LoggedInUser { get; set; }


        private readonly TourReservationService _tourReservationService;
        private readonly LocationService _locationService;
        private readonly CheckPointService _checkPointService;
        private readonly TourDateService _tourDateService;
        private readonly TourImageService _imageService;
        private readonly UserService _userService;
        private readonly TourService _tourService;
        private readonly TourAttendanceService _tourAttendanceService;
        //search,Review,Voucher,MyLocation
        private ICommand _searchCommand;
        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new RelayCommand(
                        param => this.Search_Click(),
                        param => this.CanSearchClick()
                    );
                }
                return _searchCommand;
            }
        }
        public bool CanSearchClick()
        {
            return true;
        }

        private ICommand _reviewCommand;
        public ICommand ReviewCommand
        {
            get
            {
                if (_reviewCommand == null)
                {
                    _reviewCommand = new RelayCommand(
                        param => this.Review_Click(),
                        param => this.CanReviewClick()
                    );
                }
                return _reviewCommand;
            }
        }
        public bool CanReviewClick()
        {
            return true;
        }

        private ICommand _voucherCommand;
        public ICommand VoucherCommand
        {
            get
            {
                if (_voucherCommand == null)
                {
                    _voucherCommand = new RelayCommand(
                        param => this.Voucher_Click(),
                        param => this.CanVoucherClick()
                    );
                }
                return _voucherCommand;
            }
        }
        public bool CanVoucherClick()
        {
            return true;
        }

        private ICommand _myLocationCommand;
        public ICommand MyLocationCommand
        {
            get
            {
                if (_myLocationCommand == null)
                {
                    _myLocationCommand = new RelayCommand(
                        param => this.Location_Click(),
                        param => this.CanMyLocationClick()
                    );
                }
                return _myLocationCommand;
            }
        }
        public bool CanMyLocationClick()
        {
            return true;
        }
        private ICommand _loadedCommand;
        public ICommand LoadedCommand
        {
            get
            {
                if (_loadedCommand == null)
                {
                    _loadedCommand = new RelayCommand(
                        param => this.Loaded(),
                        param => this.CanLoad()
                    );
                }
                return _loadedCommand;
            }
        }
        public bool CanLoad()
        {
            return true;
        }

        private ICommand _logoutCommand;
        public ICommand LogoutCommand
        {
            get
            {
                if (_logoutCommand == null)
                {
                    _logoutCommand = new RelayCommand(
                        param => this.Logout(),
                        param => this.CanLogout()
                    );
                }
                return _logoutCommand;
            }
        }
        public bool CanLogout()
        {
            return true;
        }
        public Guest2OverviewViewModel(User user,Window w)
        {
            
            LoggedInUser = user;
            window = w;

            _tourReservationService = new TourReservationService();
            _locationService = new LocationService();
            _checkPointService = new CheckPointService();
            _tourDateService = new TourDateService();
            _userService = new UserService();
            _imageService = new TourImageService();
            _tourService = new TourService();
            _tourAttendanceService = new TourAttendanceService();
            TourAttendances = new List<TourAttendance>(_tourAttendanceService.GetAll());

            TourReservations = new List<TourReservation>(_tourReservationService.GetAll());
            AllTours = new List<Tour>(_tourService.GetAll());
            Tours = new ObservableCollection<Tour>(_tourReservationService.GetReservedToursByUser(user, AllTours));
            PopulateTours();
            PopulateTourAttendances();
            GetSortedCheckPoints();



        }

        private void PopulateTourAttendances()
        {
            foreach (var attendance in TourAttendances)
            {
                foreach (var tour in Tours)
                {
                    if (tour.Id == attendance.Tour.Id)
                    {
                        attendance.Tour.Name = tour.Name;
                    }
                }
            }
        }

        private void PopulateTours()
        {
            foreach (var tour in AllTours)
            {
                tour.TourLocation = _locationService.GetById(tour.TourLocation.Id);
                tour.CheckPoints = _checkPointService.GetByParentId(tour.Id);
                tour.Guide = _userService.GetById(tour.Guide.Id);
                tour.TourDates = _tourDateService.GetByParentId(tour.Id);
                tour.Images = _imageService.GetByParentId(tour.Id);
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
        
        private void GetNotifications()
        {
            foreach (var attendance in TourAttendances)
            {
                if (attendance.User.Id.Equals(LoggedInUser.Id) && attendance.Present == Presence.GuideConfirmed)
                {
                    MessageBoxResult result = MessageBox.Show($"Are you present on {attendance.Tour.Name}?", "Confirm your presence", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        attendance.Present = Presence.GuestConfirmed;
                        _tourAttendanceService.Update(attendance);
                    }
                    else
                    {
                        attendance.Present = Presence.GuestNotPresent;
                        _tourAttendanceService.Update(attendance);
                    }
                }
            }
        }
        
        
        
        private void Logout()
        {
            MessageBoxResult result = ConfirmLogout();
            if (result == MessageBoxResult.Yes)
            {
                System.Windows.Application.Current.Windows.OfType<Window>()
                    .Where(w => w != window)
                    .ToList()
                    .ForEach(w => w.Close());

                LogInForm logInForm = new LogInForm();
                logInForm.Show();

                window.Close();
            }
        }
       

        private static MessageBoxResult ConfirmLogout()
        {
            string sMessageBoxText = "Are you sure you want to logout?";
            string sCaption = "Logout";

            MessageBoxButton btnMessageBox = MessageBoxButton.YesNo;
            MessageBoxImage icnMessageBox = MessageBoxImage.Question;

            MessageBoxResult result = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
            return result;
        }

        
        private void Search_Click()
        {/*
            ExploreTours searchTourWindow = new ExploreTours(LoggedInUser);
            searchTourWindow.Show();
            */
        }

        private void Review_Click()
        {
          //  MyReviews form = new MyReviews(LoggedInUser);
          //  form.Show();
        }

        private void Voucher_Click()
        {
           // MyVouchers form = new MyVouchers(LoggedInUser);
            //form.Show();
        }

        private void Location_Click()
        {
           ShowCheckPoint form = new ShowCheckPoint(LoggedInUser);
            form.Show();
        }


        
        private void Loaded()
        {
            
                GetNotifications();
        
        }
        

    }
}
