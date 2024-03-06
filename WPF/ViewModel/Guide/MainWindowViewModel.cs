using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.WPF.View.Guide;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MainPanel = SimsProject.WPF.View.Guide.MainPanel;

namespace SimsProject.WPF.ViewModel.Guide
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region PROPERTIES

        public static ObservableCollection<Tour> Tours { get; set; }
        public List<TourRequest> FilteredTourRequests { get; set; }
        public List<TourRequest> TourRequests { get; set; }
        public UserControl CurrentUserControl { get; set; }
        public User Guide { get; set; }

        private Dictionary<string, int> _languageCounts;
        private Dictionary<string, int> _locationCounts;
        private string _mostWantedLanguage;
        private string _mostWantedLocation;
        private MessageBoxResult _result;

        #endregion

        #region SERVICES

        private ITourService _tourService;
        private IUserService _userService;
        private IGuideService _guideService;
        private ILocationService _locationService;
        private ITourRequestService _tourRequestService;

        #endregion

        #region COMMANDS

        public ICommand OpenTourRequestStatisticsCommand { get; set; }
        public ICommand OpenComplexTourRequestsCommand { get; set; }
        public ICommand OpenLiveTourTrackingCommand { get; set; }
        public ICommand OpenMostVisitedTourCommand { get; set; }
        public ICommand OpenTourStatisticsCommand { get; set; }
        public ICommand OpenTourRequestsCommand { get; set; }
        public ICommand OpenTourReviewsCommand { get; set; }
        public ICommand OpenCreateTourCommand { get; set; }
        public ICommand OpenCancelTourCommand { get; set; }
        public ICommand WindowLoadedCommand { get; set; }
        public ICommand LogoutCommand { get; set; }
        public ICommand ResignCommand { get; set; }

        #endregion

        #region EVENTS

        public static event EventHandler<ActionCompletedEventArgs> ActionCompleted;
        public event EventHandler<OpenWindowEventArgs> OpenWindowRequested;
        public event EventHandler LogoutRequested;
        public event EventHandler<string> ShowMessageBox;
        public event EventHandler<string> ShowMessageBoxResult;
        public event EventHandler<MessageBoxResult> MessageBoxClosed;

        #endregion

        public MainWindowViewModel(User guide)
        {
            Guide = guide;

            InitializePanel();
            InitializeServices();
            InitializeCollections();
            InitializeCommands();
            FilterTourReviews();
        }

        private void InitializePanel()
        {
            var mainPanelViewModel = new MainPanelViewModel(Guide);
            CurrentUserControl = new MainPanel(mainPanelViewModel);
        }

        private void InitializeCommands()
        {
            OpenTourRequestStatisticsCommand = new RelayCommand(OpenTourRequestStatistics);
            OpenComplexTourRequestsCommand = new RelayCommand(OpenComplexTourRequests);
            OpenLiveTourTrackingCommand = new RelayCommand(OpenLiveTourTracking);
            OpenMostVisitedTourCommand = new RelayCommand(OpenMostVisitedTour);
            WindowLoadedCommand = new RelayCommand(RecommendToursForCreation);
            OpenTourStatisticsCommand = new RelayCommand(OpenTourStatistics);
            OpenTourRequestsCommand = new RelayCommand(OpenTourRequests);
            OpenTourReviewsCommand = new RelayCommand(OpenTourReviews);
            OpenCreateTourCommand = new RelayCommand(OpenCreateTour);
            OpenCancelTourCommand = new RelayCommand(OpenCancelTour);
            LogoutCommand = new RelayCommand(Logout);
            ResignCommand = new RelayCommand(Resign);
        }

        private void InitializeServices()
        {
            _tourRequestService = Injector.CreateInstance<ITourRequestService>();
            _locationService = Injector.CreateInstance<ILocationService>();
            _guideService = Injector.CreateInstance<IGuideService>();
            _tourService = Injector.CreateInstance<ITourService>();
            _userService = Injector.CreateInstance<IUserService>();
        }

        private void InitializeCollections()
        {
            Tours = new ObservableCollection<Tour>(_tourService.GetByGuide(Guide));
            TourRequests = new List<TourRequest>(_tourRequestService.GetAll());
            FilteredTourRequests = new List<TourRequest>();
            _languageCounts = new Dictionary<string, int>();
            _locationCounts = new Dictionary<string, int>();
        }

        private void OpenCreateTour(object obj)
        {
            var createTour = new CreateTour(Guide, null, null, null);
            OpenWindowRequested?.Invoke(this, new OpenWindowEventArgs(createTour));
            OnActionCompleted();
        }

        private void OpenTourRequestStatistics(object obj)
        {
            var tourRequestStatistics = new TourRequestStatistics(Guide);
            OpenWindowRequested?.Invoke(this, new OpenWindowEventArgs(tourRequestStatistics));
            OnActionCompleted();
        }

        private void OpenLiveTourTracking(object obj)
        {
            var exists = _tourService.ToursScheduledToday();
            if (exists)
            {
                var liveTourTracking = new LiveTourTracking(Guide);
                OpenWindowRequested?.Invoke(this, new OpenWindowEventArgs(liveTourTracking));
                OnActionCompleted();
            }
            else
            {
                const string sMessageBoxText = "There are no tours scheduled for today";
                OnShowMessageBox(sMessageBoxText);
            }
        }

        private void OpenCancelTour(object obj)
        {
            var mainPanelViewModel = (MainPanelViewModel)((MainPanel)CurrentUserControl).DataContext;
            if (mainPanelViewModel.SelectedTour is null)
            {
                const string sMessageBoxText = "Selected tour you want to cancel";
                OnShowMessageBox(sMessageBoxText);
            }
            else
            {
                var cancelTour = new CancelTour(mainPanelViewModel.SelectedTour, Guide);
                OpenWindowRequested?.Invoke(this, new OpenWindowEventArgs(cancelTour));
                OnActionCompleted();
            }
        }

        private void OpenMostVisitedTour(object obj)
        {
            var mostVisitedTour = new MostVisitedTour(Guide);
            OpenWindowRequested?.Invoke(this, new OpenWindowEventArgs(mostVisitedTour));
            OnActionCompleted();
        }

        private void OpenTourStatistics(object obj)
        {
            var tourStatistics = new TourStatistics(Guide);
            OpenWindowRequested?.Invoke(this, new OpenWindowEventArgs(tourStatistics));
            OnActionCompleted();
        }

        private void OpenTourReviews(object obj)
        {
            var tourReviews = new TourReviews(Guide);
            OpenWindowRequested?.Invoke(this, new OpenWindowEventArgs(tourReviews));
            OnActionCompleted();
        }

        private void OpenTourRequests(object obj)
        {
            var tourRequests = new TourRequests(Guide);
            OpenWindowRequested?.Invoke(this, new OpenWindowEventArgs(tourRequests));
            OnActionCompleted();
        }

        private void OpenComplexTourRequests(object obj)
        {
            var complexTourRequests = new ComplexTourRequests(Guide);
            OpenWindowRequested?.Invoke(this, new OpenWindowEventArgs(complexTourRequests));
            OnActionCompleted();
        }

        private void Logout(object obj)
        {
            OnShowMessageBoxResult("Are you sure you want to log out?");
            if (_result == MessageBoxResult.Yes)
            {
                LogoutRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnShowMessageBox(string sMessageBoxText)
        {
            ShowMessageBox?.Invoke(this, sMessageBoxText);
        }

        private void OnShowMessageBoxResult(string sMessageBoxText)
        {
            ShowMessageBoxResult?.Invoke(this, sMessageBoxText);
        }

        public void ConfirmAction(MessageBoxResult result)
        {
            MessageBoxClosed?.Invoke(this, result);
            _result = result;
        }

        private void OnActionCompleted()
        {
            var args = new ActionCompletedEventArgs();
            ActionCompleted?.Invoke(this, args);
        }

        private void Resign(object obj)
        {
            OnShowMessageBoxResult("Are you sure you want to resign?");
            if (_result != MessageBoxResult.Yes) return;

            _guideService.Resign(Guide);
            _userService.Delete(Guide);
            OnShowMessageBox("You have successfully resigned!");

            LogoutRequested?.Invoke(this, EventArgs.Empty);
        }

        private void FilterTourReviews()
        {
            foreach (var request in TourRequests.Where(IsWithinLastYear))
            {
                FilteredTourRequests.Add(request);
            }
        }

        private static bool IsWithinLastYear(TourRequest request)
        {
            var tourDate = request.FromDate.Value.Date;
            var today = DateTime.Today;
            var difference = today - tourDate;
            return difference <= TimeSpan.FromDays(365);
        }

        private void FindMostWantedLocationAndLanguage()
        {
            foreach (var request in TourRequests)
            {
                if (_languageCounts.ContainsKey(request.Language))
                {
                    _languageCounts[request.Language]++;
                }
                else
                {
                    _languageCounts[request.Language] = 1;
                }

                if (_locationCounts.ContainsKey(request.Location.ToString()))
                {
                    _locationCounts[request.Location.ToString()]++;
                }
                else
                {
                    _locationCounts[request.Location.ToString()] = 1;
                }
            }

            _mostWantedLanguage = _languageCounts.OrderByDescending(x => x.Value).FirstOrDefault().Key;
            _mostWantedLocation = _locationCounts.OrderByDescending(x => x.Value).FirstOrDefault().Key;
        }

        private void RecommendToursForCreation(object obj)
        {
            FindMostWantedLocationAndLanguage();

            if (_languageCounts[_mostWantedLanguage] > _locationCounts[_mostWantedLocation])
            {
                /*var result = MessageBox.Show($"Do you want to create tour based on {_mostWantedLanguage}?", "Most wanted language", MessageBoxButton.YesNo);

                if (result != MessageBoxResult.Yes) return;
                var createTour = new CreateTour(Guide, null, null, _mostWantedLanguage);
                createTour.Show();
                MainWindow.Close();*/
            }
            else if (_locationCounts[_mostWantedLocation] > _languageCounts[_mostWantedLanguage])
            {
                /*var location = _locationService.GetById(FilteredTourRequests.Find(r => r.Location.ToString().Equals(_mostWantedLocation)).Location.Id);

                var result = MessageBox.Show($"Do you want to create tour based on {location}?", "Most wanted location", MessageBoxButton.YesNo);

                if (result != MessageBoxResult.Yes) return;
                var createTour = new CreateTour(Guide, null, location, null);
                createTour.Show();
                MainWindow.Close();*/
            }
            else
            {
                /*var location = _locationService.GetById(FilteredTourRequests.Find(r => r.Location.ToString().Equals(_mostWantedLocation)).Location.Id);

                var result = MessageBox.Show($"Do you want to create tour based on {location} and {_mostWantedLanguage}?", "Most wanted location", MessageBoxButton.YesNo);

                if (result != MessageBoxResult.Yes) return;
                var createTour = new CreateTour(Guide, null, location, _mostWantedLanguage);
                createTour.Show();
                MainWindow.Close();*/
            }
        }
    }
}
