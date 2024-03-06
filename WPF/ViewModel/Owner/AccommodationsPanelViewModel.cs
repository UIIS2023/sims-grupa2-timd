using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Navigation;
using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.WPF.ViewModel.Domain;

namespace SimsProject.WPF.ViewModel.Owner
{
    public class AccommodationsPanelViewModel : ViewModelBase
    {
        public User Owner { get; set; }
        public NavigationService NavigationService { get; set; }
        public ObservableCollection<AccommodationViewModel> Accommodations { get; set; }
        public ObservableCollection<string> Years { get; set; }
        public ObservableCollection<AccommodationStatisticViewModel> Statistics { get; set; }

        #region SERVICES

        private IAccommodationService _accommodationService;
        private IAccommodationStatisticsService _statisticsService;

        #endregion

        #region PROPERTIES

        private AccommodationViewModel _selectedAccommodation;
        public AccommodationViewModel SelectedAccommodation
        {
            get => _selectedAccommodation;
            set
            {
                if (value != _selectedAccommodation)
                {
                    _selectedAccommodation = value;
                    _isAccommodationSelected = _selectedAccommodation is not null;
                    _imageCarouselViewModel = _selectedAccommodation is not null
                        ? new ImageCarouselViewModel(SelectedAccommodation.Images.ToList())
                        : null;
                    OnPropertyChanged(nameof(SelectedAccommodation));
                    OnPropertyChanged(nameof(IsAccommodationSelected));
                    OnPropertyChanged(nameof(ImageCarouselViewModel));
                    ChangeActiveYears();
                }
            }
        }

        private bool _isAccommodationSelected;
        public bool IsAccommodationSelected
        {
            get => _isAccommodationSelected;
            set
            {
                if (value != _isAccommodationSelected)
                {
                    _isAccommodationSelected = value;
                    OnPropertyChanged(nameof(IsAccommodationSelected));
                }
            }
        }

        private ImageCarouselViewModel _imageCarouselViewModel;
        public ImageCarouselViewModel ImageCarouselViewModel
        {
            get => _imageCarouselViewModel;
            set
            {
                if (value != _imageCarouselViewModel)
                {
                    _imageCarouselViewModel = value;
                    OnPropertyChanged(nameof(ImageCarouselViewModel));
                }
            }
        }

        private string _selectedYear;
        public string SelectedYear
        {
            get => _selectedYear;
            set
            {
                if (value != _selectedYear)
                {
                    _selectedYear = value;
                    _isYearSelected = _selectedYear is not null and not "None";
                    OnPropertyChanged(nameof(SelectedYear));
                    OnPropertyChanged(nameof(IsYearSelected));
                    ChangeStatistics();
                }
            }
        }

        private bool _isYearSelected;
        public bool IsYearSelected
        {
            get => _isYearSelected;
            set
            {
                if (value != _isYearSelected)
                {
                    
                    _isYearSelected = value;
                    OnPropertyChanged(nameof(IsYearSelected));
                }
            }
        }

        private bool _hasReservations;
        public bool HasReservations
        {
            get => _hasReservations;
            set
            {
                if (value != _hasReservations)
                {

                    _hasReservations = value;
                    OnPropertyChanged(nameof(HasReservations));
                }
            }
        }

        private int _mostOccupied;
        public int MostOccupied
        {
            get => _mostOccupied;
            set
            {
                if (value != _mostOccupied)
                {

                    _mostOccupied = value;
                    OnPropertyChanged(nameof(MostOccupied));
                }
            }
        }

        #endregion

        #region COMMANDS

        public ICommand RegisterCommand { get; set; }
        public ICommand RenovateCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        #endregion

        #region EVENTS

        public bool ConfirmActionResult { get; set; }
        public event EventHandler ConfirmActionRequested;

        #endregion

        public AccommodationsPanelViewModel(User owner, NavigationService navigationService)
        {
            Owner = owner;
            NavigationService = navigationService;

            InitializeServices();
            InitializeCollections();
            InitializeCommands();
        }

        private void InitializeServices()
        {
            _accommodationService = Injector.CreateInstance<IAccommodationService>();
            _statisticsService = Injector.CreateInstance<IAccommodationStatisticsService>();
        }

        private void InitializeCollections()
        {
            Accommodations = new ObservableCollection<AccommodationViewModel>();
            foreach (var accommodation in _accommodationService.GetByOwner(Owner))
            {
                Accommodations.Add(new AccommodationViewModel(accommodation));
            }

            Years = new ObservableCollection<string>();
            Statistics = new ObservableCollection<AccommodationStatisticViewModel>();
        }

        private void InitializeCommands()
        {
            RegisterCommand = new RelayCommand(Register);
            RenovateCommand = new RelayCommand(Renovate, CanDoMoreOptions);
            DeleteCommand = new RelayCommand(Delete, CanDoMoreOptions);
        }

        private void Register(object obj)
        {
            NavigationService.Navigate(new Uri("../../WPF/View/Owner/RegistrationPanel.xaml", UriKind.Relative));
        }

        private void Renovate(object obj)
        {
            NavigationService.Navigate(new Uri("../../WPF/View/Owner/RenovationsPanel.xaml", UriKind.Relative));
        }

        private void Delete(object obj)
        {
            ConfirmActionRequested?.Invoke(this, EventArgs.Empty);
            if (!ConfirmActionResult) return;

            _accommodationService.DeleteAccommodation(SelectedAccommodation.Id);
            Accommodations.Remove(SelectedAccommodation);
            SelectedAccommodation = null;
        }

        private bool CanDoMoreOptions(object obj)
        {
            return SelectedAccommodation is not null;
        }

        private void ChangeStatistics()
        {
            if (SelectedAccommodation is null) return;

            Statistics.Clear();
            int? year = SelectedYear is "None" or null ? null : int.Parse(SelectedYear);
            foreach (var statistic in _statisticsService.GetStatistics(SelectedAccommodation.Id, year))
            {
                Statistics.Add(new AccommodationStatisticViewModel(statistic));
            }
            MostOccupied = _statisticsService.GetMostOccupied(SelectedAccommodation.Id, year);
        }

        private void ChangeActiveYears()
        {
            if(SelectedAccommodation is null ) return;
            
            var availableYears = _statisticsService.GetActiveYears(SelectedAccommodation.Id);
            if (availableYears is null)
            {
                HasReservations = false;
                return;
            }
            HasReservations = true;
            Years.Clear();
            Years.Add("None");
            foreach (var year in availableYears)
            {
                Years.Add(year.ToString());
            }
            SelectedYear = Years[0];
        }
    }
}