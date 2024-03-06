using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Navigation;
using SimsProject.Application.Interface;
using SimsProject.Domain.Model;

namespace SimsProject.WPF.ViewModel.Guest1
{
    public class CreateNewForumViewModel : ViewModelBase
    {
        public User CurrentUser;
        public NavigationService NavigationService { get; set; }
        private readonly ILocationService _locationService;
        private readonly IForumService _forumService;
        public CreateNewForumViewModel(User currentUser, NavigationService navigationService)
        {
            CurrentUser = currentUser;
            NavigationService = navigationService;
            _locationService = Injector.CreateInstance<ILocationService>();
            _forumService = Injector.CreateInstance<IForumService>();
            FillCountryComboBox();
            IsCityEnabled = false;
            CreateCommand = new RelayCommand(CreateClick, CanCreateClick);
            GoBackCommand = new RelayCommand(GoBackClick);
        }
        public ICommand GoBackCommand { get; set; }
        public void GoBackClick(object obj)
        {
            NavigationService.GoBack();
        }
        public ICommand CreateCommand { get; set; }
        public bool CanCreateClick(object obj)
        {
            return !string.IsNullOrEmpty(Text) && SelectedCityIndex != -1;
        }
        public void CreateClick(object obj)
        {
            Forum forum = new Forum(CurrentUser, Text, _locationService.GetByCityAndCountry(Cities[SelectedCityIndex], Countries[SelectedCountryIndex]));
            _forumService.Create(forum);
            NavigationService.Navigate(
                new Uri("/SimsProject;component/WPF/View/Guest1/ForumsForm.xaml", UriKind.Relative));
        }

        private void FillCountryComboBox()
        {
            var countries = _locationService.GetDistinctCountries();
            Countries = countries;
            SelectedCountryIndex = -1;
        }
        private void OnCountrySelectionChanged()
        {
            List<string> cities;
            IsCityEnabled = true;
            var c = "";
            var countries = _locationService.GetDistinctCountries();
            if (SelectedCountryIndex != -1) c = countries[SelectedCountryIndex];
            else
            {
                SelectedCityIndex = -1;
                return;
            }
            cities = _locationService.GetAllCitiesByCountry(c);
            Cities = cities;
            SelectedCityIndex = -1;
        }

        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        private List<string> _countries;
        public List<string> Countries
        {
            get => _countries;
            set
            {
                if (_countries != value)
                {
                    _countries = value;
                    OnPropertyChanged(nameof(Countries));
                }
            }
        }

        private List<string> _cities;
        public List<string> Cities
        {
            get => _cities;
            set
            {
                if (_cities != value)
                {
                    _cities = value;
                    OnPropertyChanged(nameof(Cities));
                }
            }
        }

        private bool _isCityEnabled = true;
        public bool IsCityEnabled
        {
            get => _isCityEnabled;
            set
            {
                _isCityEnabled = value;
                OnPropertyChanged(nameof(IsCityEnabled));
            }
        }

        private string _selectedCountry;
        public string SelectedCountry
        {
            get => _selectedCountry;
            set
            {
                _selectedCountry = value;
                OnPropertyChanged(nameof(SelectedCountry));
            }
        }

        private string _selectedCity;
        public string SelectedCity
        {
            get => _selectedCity;
            set
            {
                _selectedCity = value;
                OnPropertyChanged(nameof(SelectedCity));
            }
        }

        private int _selectedCountryIndex;
        public int SelectedCountryIndex
        {
            get => _selectedCountryIndex;
            set
            {
                _selectedCountryIndex = value;
                OnPropertyChanged(nameof(SelectedCountryIndex));
                OnCountrySelectionChanged();
            }
        }

        private int _selectedCityIndex;
        public int SelectedCityIndex
        {
            get => _selectedCityIndex;
            set
            {
                _selectedCityIndex = value;
                OnPropertyChanged(nameof(SelectedCityIndex));
            }
        }
    }
}
