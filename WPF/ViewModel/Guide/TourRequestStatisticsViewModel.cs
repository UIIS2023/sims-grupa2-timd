using LiveCharts;
using LiveCharts.Wpf;
using SimsProject.Application.Interface;
using SimsProject.WPF.ViewModel.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace SimsProject.WPF.ViewModel.Guide
{
    public class TourRequestStatisticsViewModel : ViewModelBase
    {
        #region PROPERTIES

        public SeriesCollection StatisticsChart { get; set; }
        public List<TourRequestViewModel> TourRequests { get; set; }
        public ObservableCollection<string> Countries { get; set; }
        public ObservableCollection<string> Cities => new(_tourRequestService.FindDistinctCitiesByCountry(Country));
        public List<string> Languages { get; set; }
        public List<string> Years { get; set; }
        public List<string> Colors { get; set; }

        public event EventHandler<string> ShowMessageBox;

        private string _selectedYear;
        private string _language;
        private string _country;
        private string _city;
        private string _statisticsLabel;
        private bool _isCityEnabled;
        private bool _isCountryEnabled = true;
        private bool _isLanguageEnabled = true;

        public string StatisticsLabel
        {
            get => _statisticsLabel;
            set
            {
                if (value != _statisticsLabel)
                {
                    _statisticsLabel = value;
                    OnPropertyChanged(nameof(StatisticsLabel));
                }
            }
        }

        public string SelectedYear
        {
            get => _selectedYear;
            set
            {
                if (value != _selectedYear)
                {
                    _selectedYear = value;
                    OnPropertyChanged(nameof(SelectedYear));
                }
            }
        }

        public string Language
        {
            get => _language;
            set
            {
                if (value != _language)
                {
                    _language = value;
                    OnPropertyChanged(nameof(Language));
                    if (_language != null)
                    {
                        SetIsEnabled(false, false, true);
                    }
                    else
                    {
                        SetIsEnabled(true, false, true);
                    }
                }
            }
        }

        public string City
        {
            get => _city;
            set
            {
                if (value != _city)
                {
                    _city = value;
                    OnPropertyChanged(nameof(City));
                }
            }
        }
        public string Country
        {
            get => _country;
            set
            {
                if (value != _country)
                {
                    _country = value;
                    OnPropertyChanged(nameof(Country));
                    OnPropertyChanged(nameof(Cities));
                    if (_country != null)
                    {
                        SetIsEnabled(true, true, false);
                    }
                    else
                    {
                        SetIsEnabled(true, false, true);
                    }
                }
            }
        }

        public bool IsCityEnabled
        {
            get => _isCityEnabled;
            set
            {
                _isCityEnabled = value;
                OnPropertyChanged(nameof(IsCityEnabled));
            }
        }


        public bool IsCountryEnabled
        {
            get => _isCountryEnabled;
            set
            {
                _isCountryEnabled = value;
                OnPropertyChanged(nameof(IsCountryEnabled));
            }
        }

        public bool IsLanguageEnabled
        {
            get => _isLanguageEnabled;
            set
            {
                _isLanguageEnabled = value;
                OnPropertyChanged(nameof(IsLanguageEnabled));
            }
        }

        #endregion

        #region COMMANDS

        public ICommand OpenGuideOverviewCommand { get; set; }
        public ICommand OpenGetStatisticsCommand { get; set; }
        public ICommand ClearParametersCommand { get; set; }

        #endregion

        #region SERVICES

        private ITourRequestService _tourRequestService;

        #endregion

        public TourRequestStatisticsViewModel()
        {
            InitializeServices();
            InitializeCollections();
            InitializeCommands();
            FindDistinctLanguages();
            FindDistinctYears();
        }

        private void InitializeServices()
        {
            _tourRequestService = Injector.CreateInstance<ITourRequestService>();
        }

        private void InitializeCollections()
        {
            Colors = new List<string>() { "#78866B", "#A9BA9D", "#D0D9CD", "#B2BEB5", "#828E84", "#687169", "#AEC2B6", "#7C9070", "#609966", "#9DC08B", "#557153", "#A4BE7B", "#C7E9B0" };
            Countries = new ObservableCollection<string>(_tourRequestService.FindDistinctCountries());
            TourRequests = new List<TourRequestViewModel>();
            StatisticsChart = new SeriesCollection();
            Languages = new List<string>();
            Years = new List<string>();

            foreach (var request in _tourRequestService.GetAll())
            {
                TourRequests.Add(new TourRequestViewModel(request));
            }
        }

        private void InitializeCommands()
        {
            OpenGuideOverviewCommand = new RelayCommand(OpenGuideOverview);
            OpenGetStatisticsCommand = new RelayCommand(DisplayStatistics);
            ClearParametersCommand = new RelayCommand(ClearParameters);
        }

        private void FindDistinctYears()
        {
            Years.Add("All years");

            foreach (var year in _tourRequestService.FindDistinctYears())
            {
                Years.Add(year);
            }

            SelectedYear = Years[0];
        }

        private void FindDistinctLanguages()
        {
            foreach (var language in _tourRequestService.FindDistinctLanguages())
            {
                Languages.Add(language);
            }
        }

        private void SetIsEnabled(bool countryEnabled, bool cityEnabled, bool languageEnabled)
        {
            IsCountryEnabled = countryEnabled;
            IsCityEnabled = cityEnabled;
            IsLanguageEnabled = languageEnabled;
        }

        private void ClearParameters(object obj)
        {
            Language = null;
            Country = null;
            City = null;
        } 

        private void DisplayStatistics(object obj)
        {
            if (Language is null && Country is null && City is null)
            {
                const string sMessageBoxText = "You need to select at least one parameter";
                OnShowMessageBox(sMessageBoxText);
            }
            else if (Language is not null && Country is null && City is null)
            {
                StatisticsChart.Clear();
                DisplayLanguageStatistics();
            }
            else if (Language is null && Country is not null && City is null)
            {
                const string sMessageBoxText = "You need to select a city for the specific country";
                OnShowMessageBox(sMessageBoxText);
            }
            else
            {
                StatisticsChart.Clear();
                DisplayLocationStatistics();
            }
        }

        private void DisplayLocationStatistics()
        {
            StatisticsLabel = "Location statistics for " + Country + ", " + City + " in year " + SelectedYear;
            if (SelectedYear.Equals("All years"))
            {
                var groupedRequests = TourRequests.Where(r => r.FromDate.HasValue).GroupBy(r => r.FromDate.Value.Year);

                CreateChartForAllYears(groupedRequests);
            }
            else
            {
                var year = int.Parse(SelectedYear);
                var location = $"{City}, {Country}";

                var groupedRequests = TourRequests.Where(r => r.FromDate.HasValue && r.FromDate.Value.Year == year && r.Location.ToString() == location).GroupBy(r => r.FromDate.Value.Month);

                CreateChartForSpecificYear(groupedRequests);
            }
        }

        private void DisplayLanguageStatistics()
        {
            StatisticsLabel = "Language statistics for " + Language + " in year " + SelectedYear;
            if (SelectedYear.Equals("All years"))
            {
                var groupedRequests = TourRequests.Where(r => r.FromDate.HasValue).GroupBy(r => r.FromDate.Value.Year);

                CreateChartForAllYears(groupedRequests);
            }
            else
            {
                var year = int.Parse(SelectedYear);

                var groupedRequests = TourRequests.Where(r => r.FromDate.HasValue && r.FromDate.Value.Year == year).GroupBy(r => r.FromDate.Value.Month);

                CreateChartForSpecificYear(groupedRequests);
            }
        }

        private void CreateChartForAllYears(IEnumerable<IGrouping<int, TourRequestViewModel>> groupedRequests)
        {
            var colorIndex = 0;

            foreach (var group in groupedRequests)
            {
                var color = Colors[colorIndex % Colors.Count];
                colorIndex++;

                StatisticsChart.Add(new PieSeries
                {
                    Title = group.Key.ToString(),
                    Values = new ChartValues<double> { group.Count() },
                    DataLabels = true,
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color)!),
                    LabelPoint = point => $"{point.Y} ({point.Participation:P0})"
                });
            }
        }

        private void CreateChartForSpecificYear(IEnumerable<IGrouping<int, TourRequestViewModel>> groupedRequests)
        {
            var colorIndex = 0;

            foreach (var group in groupedRequests)
            {
                var color = Colors[colorIndex % Colors.Count];
                colorIndex++;

                var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(group.Key);

                StatisticsChart.Add(new PieSeries
                {
                    Title = monthName,
                    Values = new ChartValues<double> { group.Count() },
                    DataLabels = true,
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color)!),
                    LabelPoint = point => $"{point.Y} ({point.Participation:P0})"
                });
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
    }
}
