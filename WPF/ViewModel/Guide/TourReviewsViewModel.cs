using SimsProject.Domain.Model;
using System.Collections.Generic;
using SimsProject.WPF.ViewModel.Domain;
using System.Collections.ObjectModel;
using SimsProject.Application.Interface;
using System.Windows.Input;
using Image = SimsProject.Domain.Model.Image;
using System.Windows;
using System;

namespace SimsProject.WPF.ViewModel.Guide
{
    public class TourReviewsViewModel : ViewModelBase
    {
        #region PROPERTIES

        public List<TourAttendanceViewModel> TourAttendances { get; set; }
        public List<CheckPointViewModel> CheckPoints { get; set; }
        public List<TourReviewViewModel> Reviews { get; set; }
        public List<TourDateViewModel> TourDates { get; set; }
        public List<TourViewModel> FinishedTours { get; set; }
        public List<TourViewModel> Tours { get; set; }
        public List<Image> Images { get; set; }

        private MessageBoxResult _result;

        private ObservableCollection<TourReviewViewModel> _filteredReviews;
        public ObservableCollection<TourReviewViewModel> FilteredReviews
        {
            get => _filteredReviews;
            set
            {
                _filteredReviews = value;
                OnPropertyChanged(nameof(FilteredReviews));
            }
        }

        private TourViewModel _selectedTour;
        public TourViewModel SelectedTour
        {
            get => _selectedTour;
            set
            {
                if (value != _selectedTour)
                {
                    _selectedTour = value;
                    OnPropertyChanged(nameof(SelectedTour));
                }
            }
        }

        private TourReviewViewModel _selectedReview;
        public TourReviewViewModel SelectedReview
        {
            get => _selectedReview;
            set
            {
                if (_selectedReview != value)
                {
                    _selectedReview = value;
                    OnPropertyChanged(nameof(SelectedReview));
                }
            }
        }
        private User LoggedInUser { get; set; }

        #endregion

        #region COMMANDS
        public ICommand OpenGuideOverviewCommand { get; set; }
        public ICommand ReportReviewCommand { get; set; }
        public ICommand FindTourCommand { get; set; }

        #endregion

        #region SERVICES

        private ITourAttendanceService _tourAttendanceService;
        private ICheckPointService _checkPointService;
        private ITourDateService _tourDateService;
        private ITourService _tourService;
        private ITourReviewService _tourReviewService;

        #endregion

        #region EVENTS

        public event EventHandler<string> ShowMessageBox;
        public event EventHandler<string> ShowMessageBoxResult;
        public event EventHandler<MessageBoxResult> MessageBoxClosed;

        #endregion

        public TourReviewsViewModel(User user)
        {
            LoggedInUser = user;

            InitializeServices();
            InitializeCollections();
            InitializeCommands();
        }

        private void InitializeServices()
        {
            _tourAttendanceService = Injector.CreateInstance<ITourAttendanceService>();
            _checkPointService = Injector.CreateInstance<ICheckPointService>();
            _tourReviewService = Injector.CreateInstance<ITourReviewService>();
            _tourDateService = Injector.CreateInstance<ITourDateService>();
            _tourService = Injector.CreateInstance<ITourService>();
        }

        private void InitializeCommands()
        {
            OpenGuideOverviewCommand = new RelayCommand(OpenGuideOverview);
            ReportReviewCommand = new RelayCommand(ReportReview);
            FindTourCommand = new RelayCommand(FindTour);
        }

        private void InitializeCollections()
        {
            FilteredReviews = new ObservableCollection<TourReviewViewModel>();
            TourAttendances = new List<TourAttendanceViewModel>();
            CheckPoints = new List<CheckPointViewModel>(); 
            Reviews = new List<TourReviewViewModel>(); 
            FinishedTours = new List<TourViewModel>();
            TourDates = new List<TourDateViewModel>();
            Tours = new List<TourViewModel>();

            foreach (var tourAttendance in _tourAttendanceService.GetAll())
            {
                TourAttendances.Add(new TourAttendanceViewModel(tourAttendance));
            }

            foreach (var tour in _tourService.GetByGuide(LoggedInUser))
            {
                Tours.Add(new TourViewModel(tour));
            }

            foreach (var tourDate in _tourDateService.GetAll())
            {
                TourDates.Add(new TourDateViewModel(tourDate));
            }

            foreach (var tourReview in _tourReviewService.GetAll())
            {
                Reviews.Add(new TourReviewViewModel(tourReview));
            }

            foreach (var checkPoint in _checkPointService.GetAll())
            {
                CheckPoints.Add(new CheckPointViewModel(checkPoint));
            }

            FindFinishedTours();
        }

        private void FindFinishedTours()
        {
            foreach (var tour in _tourService.FindFinishedTours())
            {
                FinishedTours.Add(new TourViewModel(tour));
            }
        }

        private void FindTour(object obj)
        {
            if (SelectedTour == null)
            {
                const string sMessageBoxText = "Please select the tour for which you want to see the reviews";
                OnShowMessageBox(sMessageBoxText);
            }
            else
            {
                FillFilteredReviewsList();

                if (FilteredReviews.Count != 0) return;

                const string sMessageBoxText = "There are no available reviews for this tour yet";
                OnShowMessageBox(sMessageBoxText);
            }
        }

        private void ReportReview(object obj)
        {
            var selectedReview = obj as TourReviewViewModel;

            if (selectedReview is { IsValid: false })
            {
                const string sMessageBoxText = "Review has already been reported";
                OnShowMessageBox(sMessageBoxText);
            }
            else
            {
                OnShowMessageBoxResult("Are you sure you want to report this review?");
                if (_result != MessageBoxResult.OK) return;

                if (selectedReview == null) return;
                selectedReview.IsValid = false;
                _tourReviewService.UpdateById(selectedReview.Id);
                FillFilteredReviewsList();
            }
        }

        private void FillFilteredReviewsList()
        {
            FilteredReviews = new ObservableCollection<TourReviewViewModel>();
            foreach (var review in _tourReviewService.GetFilteredReviews(SelectedTour.Id))
            {
                FilteredReviews.Add(new TourReviewViewModel(review));
            }
        }

        private void OpenGuideOverview(object obj)
        {
            OnActionCompleted();
        }

        public static event EventHandler<ActionCompletedEventArgs> ActionCompleted;

        private void OnActionCompleted()
        {
            var args = new ActionCompletedEventArgs();
            ActionCompleted?.Invoke(this, args);
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
    }
}
