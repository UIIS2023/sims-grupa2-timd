using SimsProject.Domain.Model;
using SimsProject.WPF.View.Guest1;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using Image = SimsProject.Domain.Model.Image;
using System.Windows.Input;
using SimsProject.Application.Interface;
using SimsProject.Application.Localization;
using SimsProject.WPF.ViewModel.Domain;
using System.Windows.Navigation;

namespace SimsProject.WPF.ViewModel.Guest1
{
    public class ReviewAccommodationViewModel : ViewModelBase, IDataErrorInfo
    {
        public User CurrentUser;
        public NavigationService NavigationService { get; set; }
        public AccommodationReservationViewModel Reservation { get; set; }
        public static ObservableCollection<Image> _images;
        private readonly IAccommodationReservationService _accommodationReservationService;
        private readonly IAccommodationReviewService _accommodationReviewService;
        public ReviewAccommodationViewModel(User currentUser, NavigationService navigationService)
        {
            CurrentUser = currentUser;
            NavigationService = navigationService;
            Reservation = ReviewsFormViewModel.SelectedAccommodationReservation;
            _images = new ObservableCollection<Image>();
            _accommodationReservationService = Injector.CreateInstance<IAccommodationReservationService>();
            _accommodationReviewService = Injector.CreateInstance<IAccommodationReviewService>();
            SubmitReviewCommand = new RelayCommand(SubmitReviewClick, CanSubmitReviewClick);
            ViewImageCommand = new RelayCommand(ViewImageClick, CanViewImageClick);
            AddImageCommand = new RelayCommand(AddImageClick, CanAddImageClick);
            GoBackCommand = new RelayCommand(GoBackClick);
        }
        public ICommand GoBackCommand { get; set; }
        public void GoBackClick(object obj)
        {
            NavigationService.GoBack();
        }
        public ICommand SubmitReviewCommand { get; set; }
        public bool CanSubmitReviewClick(object obj)
        {
            return !string.IsNullOrEmpty(CommentText) && SelectedMode != -1 && SelectedMode2 != -1; 
        }
        public void SubmitReviewClick(object obj)
        {
            if (UrgencyLevel == 0 && !string.IsNullOrEmpty(RenovationCommentText))
            {
                MessageBox.Show(TranslationSource.Instance["PickUrgencyLevel"]);
                return;
            }
            int selectedOption1 = SelectedMode, selectedOption2 = SelectedMode2;
            AccommodationReview review = new AccommodationReview(selectedOption1 + 1, selectedOption2 + 1, CommentText, Reservation.Accommodation.Owner, CurrentUser, _accommodationReservationService.GetById(Reservation.Id), null, UrgencyLevel, RenovationCommentText);
            _accommodationReviewService.CreateReview(review, Reservation.Id, _images);
            MessageBox.Show(TranslationSource.Instance["RS"]);
            NavigationService.GoBack();
        }
        public ICommand AddImageCommand { get; set; }
        public bool CanAddImageClick(object obj)
        {
            return IsUrlValid(UrlText);
        }
        public void AddImageClick(object obj)
        {
            try
            {
                Image image = new()
                {
                    Url = UrlText,
                };
                _images.Add(image);
                UrlText = "";
            }
            catch
            {
                MessageBox.Show(TranslationSource.Instance["ErrorAddingImage"], TranslationSource.Instance["Error"], MessageBoxButton.OK);
            }
        }
        private static bool IsUrlValid(string url)
        {
            if (url == null) return false;
            var pattern = @"^(http|https)://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?$";
            return Regex.IsMatch(url, pattern);
        }
        public ICommand ViewImageCommand { get; set; }
        public bool CanViewImageClick(object obj)
        {
            return _images.Count > 0;
        }
        public void ViewImageClick(object obj)
        {
            ImageViewer form = new(_images, true);
            form.ShowDialog();
        }
        private string _urlText;
        public string UrlText
        {
            get => _urlText;
            set
            {
                _urlText = value;
                OnPropertyChanged(nameof(UrlText));
            }
        }
        private string _commentText;
        public string CommentText
        {
            get => _commentText;
            set
            {
                _commentText = value;
                OnPropertyChanged(nameof(CommentText));
            }
        }
        private string _renovationCommentText;
        public string RenovationCommentText
        {
            get => _renovationCommentText;
            set
            {
                _renovationCommentText = value;
                OnPropertyChanged(nameof(RenovationCommentText));
            }
        }
        private int _urgencyLevel;
        public int UrgencyLevel
        {
            get => _urgencyLevel;
            set
            {
                _urgencyLevel = value;
                OnPropertyChanged(nameof(UrgencyLevel));
            }
        }
        private bool[] _modeArray = { false, false, false, false, true };
        public bool[] ModeArray => _modeArray;
        public int SelectedMode => Array.IndexOf(_modeArray, true);

        private bool[] _modeArray2 = { false, false, false, false, true };
        public bool[] ModeArray2 => _modeArray2;
        public int SelectedMode2 => Array.IndexOf(_modeArray2, true);
        public string Error => null;
        public string this[string columnName]
        {
            get
            {
                string error = null;

                switch (columnName)
                {
                    case nameof(CommentText):
                        if (string.IsNullOrEmpty(CommentText))
                        {
                            error = TranslationSource.Instance["ERR7"];
                        }
                        break;
                }
                return error;
            }
        }
    }
}
