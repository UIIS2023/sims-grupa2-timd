using SimsProject.Application.UseCase;
using SimsProject.Domain.Model;
using SimsProject.WPF.View.Guest2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Image = SimsProject.Domain.Model.Image;

namespace SimsProject.WPF.ViewModel.Guest2
{


    public class CreateReviewViewModel : INotifyPropertyChanged
    {

        private  Action UpdateTableDelegate;
        Page _page;
        public static ObservableCollection<Image> Images;
        public List<TourAttendance> Attendances;
        public List<TourDate> TourDates=new();
        public TourReservation SelectedReservation;

        private readonly TourReviewService tourReviewService;
        private readonly TourAttendanceService tourAttendanceService;
        private readonly TourReservationService tourReservationService;
        private readonly TourDateService tourDateService;


        public User LoggedInUser;
        

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public CreateReviewViewModel(User LoggedInUser, TourReservation selectedReservation, Page ReviewPage, Action updateTable){
            tourAttendanceService = new TourAttendanceService();
            tourReservationService = new TourReservationService();
            tourReviewService = new TourReviewService();
            tourDateService = new TourDateService();
            this.LoggedInUser = LoggedInUser;
            SelectedReservation = selectedReservation;
            Images = new ();
            _page = ReviewPage;
            UpdateTableDelegate = updateTable;

            TourDates = tourDateService.GetAll();
            Attendances = tourAttendanceService.GetAll();
           

        }

    private string _urlText;
    public string UrlText
    {
        get { return _urlText; }
        set
        {
            _urlText = value;
            OnPropertyChanged(nameof(UrlText));
        }
    }
    private string _commentText;
    public string CommentText
    {
        get { return _commentText; }
        set
        {
            _commentText = value;
            OnPropertyChanged(nameof(CommentText));
        }
    }

    private ICommand _submitReviewCommand;
    public ICommand SubmitReviewCommand
    {
        get
        {
            if (_submitReviewCommand == null)
            {
                _submitReviewCommand = new RelayCommand(
                    param => this.SubmitReviewClick(),
                    param => this.CanSubmitReviewClick()
                );
            }
            return _submitReviewCommand;
        }
    }
    public bool CanSubmitReviewClick()
    {
            return true; 
    }


        private ICommand _CancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                if (_CancelCommand == null)
                {
                    _CancelCommand = new RelayCommand(
                        param => this.CancelClick(),
                        param => this.CanCancelClick()
                    );
                }
                return _CancelCommand;
            }
        }
        public bool CanCancelClick()
        {
            return true;
        }


        private bool[] _modeArray = new bool[6];
        public bool[] ModeArray => _modeArray;

        public int SelectedKnowledge => Array.IndexOf(_modeArray, true);


        private bool[] _modeArray2 = new bool[6];
        public bool[] ModeArray2 => _modeArray2;

        public int SelectedLanguage => Array.IndexOf(_modeArray2, true);

        private bool[] _modeArray3 = new bool[6];
        public bool[] ModeArray3 => _modeArray3;

        public int SelectedInterestigness => Array.IndexOf(_modeArray3, true);

        
        public TourDate FindReservedDate() {
            TourDate t = new TourDate();
            foreach (TourDate tourDate in TourDates) {
                if (((DateTime)tourDate.Date)== SelectedReservation.Date) {
                    t= tourDate;
                }
            }
            return t;
        }
        public void SubmitReviewClick()
        {
            if ( AllChecked() && IsCommentValid() && HasImages()) {
                TourDate tourDate = FindReservedDate();
                int Knowledge = SelectedKnowledge + 1, Language = SelectedLanguage + 1, Interestigness = SelectedInterestigness + 1;
                tourReviewService.CreateTourReview(LoggedInUser, Knowledge, Language, Interestigness, CommentText, Attendances, SelectedReservation, Images, tourDate);
                UpdateTableDelegate();
                NavigateToReviews();
            }
        }
        private void CancelClick() {
            var navigationService = _page.NavigationService;
            navigationService.GoBack();

        }

        public void NavigateToReviews() {
            var navigationService = _page.NavigationService;
            var MyReviewsPage = new MyReviews(LoggedInUser);
            navigationService.Navigate(MyReviewsPage);
        }

        private bool  IsUrlValid(string url)
        {
            var pattern = @"^(http|https)://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?$";
            if (url == null || !Regex.IsMatch(url, pattern)) {
                MessageBox.Show("Invalid Url !");
                return false;
            }
            return true;
        
        }


        private ICommand _addImageCommand;
        public ICommand AddImageCommand
        {
            get
            {
                if (_addImageCommand == null)
                {
                    _addImageCommand = new RelayCommand(
                        param => this.AddImageClick(),
                        param => this.CanAddImageClick()
                    );
                }
                return _addImageCommand;
            }
        }
        public bool CanAddImageClick()
        {
            return true;
        }
        public void AddImageClick()
        {
            if (IsUrlValid(UrlText) )
            {
                try
                {
                    Image image = new()
                    {
                        Url = UrlText,
                    };
                    Images.Add(image);
                    UrlText = String.Empty;
                }
                catch (Exception exception)
                {
                    MessageBox.Show($"Error adding image from URL '{UrlText}': {exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private ICommand _viewImageCommand;

        public ICommand ViewImageCommand
        {
            get
            {
                if (_viewImageCommand == null)
                {
                    _viewImageCommand = new RelayCommand(
                        param => this.ViewImageClick(),
                        param => this.CanViewImageClick()
                    );
                }
                return _viewImageCommand;
            }
        }
        public bool CanViewImageClick()
        {
            return Images.Count > 0;
        }
        public void ViewImageClick()
        {
            ReviewImages form = new ReviewImages(Images);
            form.ShowDialog();
        }

        public bool IsCommentValid()
        {

            if (string.IsNullOrEmpty(CommentText))
            {
                MessageBox.Show("You must eneter a comment!");
                return false;
            }
            return true;
        }

        public bool AllChecked() {
            if ((SelectedKnowledge+1) == 0 || (SelectedLanguage+1) == 0 || (SelectedInterestigness+1) ==0) {
                MessageBox.Show("You have to grade each category!");
                return false;
            }
            return true;
        }

        public bool HasImages() {
            if (Images.Count == 0) {
                MessageBox.Show("You have to add at least one image");
                return false;
            }
            return true;
        }
       

    }
}
