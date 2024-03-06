using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using SimsProject.Application.Interface;
using SimsProject.Application.UseCase;
using SimsProject.Domain.Model;
using SimsProject.WPF.ViewModel.Domain;
using SimsProject.Application.DTO;

namespace SimsProject.WPF.ViewModel.Guest2
{
    public class StatisticsViewModel : ViewModelBase
    {
        User LoggedInUser;
        TourRequestService _tourRequestService;
        public ObservableCollection<TourRequest> AllRequests { get; set; }
        public ObservableCollection<TourRequest> AcceptedRequests { get; set; }
        public ObservableCollection<TourRequest> RequestsByYear { get; set; }
        public ObservableCollection<String> Languages { get; set; }
        public ObservableCollection<Location> Locations { get; set; }
        public List<int> Counts { get; set; }
        public List<int> CountsLocation { get; set; }
        public SeriesCollection StatisticsChart { get; set; }
        public SeriesCollection LanguageSeries { get; set; }
        public SeriesCollection LocationSeries { get; set; }
        Dictionary<string, int> dataCounts = new Dictionary<string, int>();
        Dictionary<string, int> dataCounts2 = new Dictionary<string, int>();

        private List<String> _yearList;
        public List<String> YearList
        {
            get { return _yearList; }
            set
            {
                _yearList = value;
                OnPropertyChanged(nameof(YearList));
            }
        }

        private string _selectedYear;
        public string SelectedYear
        {
            get { return _selectedYear; }
            set
            {
                _selectedYear = value;
                OnPropertyChanged(nameof(SelectedYear));
                ChartForSelectedYear();
               // ChartForAllYears();
            }
        }

        private int _averagePeople;
        public int AveragePeople
        {
            get { return _averagePeople; }
            set
            {
                _averagePeople = value;
                OnPropertyChanged(nameof(AveragePeople));
               
            }
        }

        public StatisticsViewModel(User currentUser) {
            LoggedInUser = currentUser;
            _tourRequestService = new();
            YearList = new();
            StatisticsChart = new();
            AllRequests = new(_tourRequestService.GetAll());
            AcceptedRequests = new();
            RequestsByYear = new();
            Languages = new(_tourRequestService.FindDistinctLanguages());
            Locations = new(_tourRequestService.FindDistinctLocations());
            Counts = new();
            CountsLocation = new();
            dataCounts = new();
            dataCounts2 = new();
            PopulateCounts();
            PopulateCountsLocation();
            FindDistinctYears();
            StatisticsChart = new();
            LanguageSeries = new();
            LocationSeries = new();
            //ChartForAllYears();
            SelectedYear = YearList[0];
            PopulateDictionaryLanguage();
            PopulateDictionaryLocation();
            LanguageSeries = GenerateChartData(dataCounts);
            LocationSeries = GenerateChartData(dataCounts2);
        }

        private void FindDistinctYears()
        {
            YearList.Add("All years");
            foreach (var year in _tourRequestService.FindDistinctYears())
            {
                YearList.Add(year);
            }
        }
        private void CreateChart()
        {
            double totalRequests;
            double acceptedPercentage;
            double notAcceptedPercentage;
            if (!SelectedYear.Equals("All years"))
            {
                AveragePeople = SumGuestNum(RequestsByYear.ToList()) / (RequestsByYear.Count);
                 totalRequests = RequestsByYear.Count;
                 acceptedPercentage = (AcceptedRequests.Count / totalRequests) * 100;
                 notAcceptedPercentage = ((RequestsByYear.Count - AcceptedRequests.Count) / totalRequests) * 100;
            }
            else
            {
                AveragePeople = SumGuestNum(AcceptedRequests.ToList()) /( AcceptedRequests.Count);
                 totalRequests = AllRequests.Count;
                 acceptedPercentage = (AcceptedRequests.Count / totalRequests) * 100;
                 notAcceptedPercentage = ((AllRequests.Count - AcceptedRequests.Count) / totalRequests) * 100;
            }

            StatisticsChart.Clear();

            StatisticsChart.Add(new PieSeries
            {
                Title = "Accepted Requests",
                Values = new LiveCharts.ChartValues<double> { acceptedPercentage },
                DataLabels = true,
                Fill = new SolidColorBrush(Colors.Green),
                LabelPoint = point => $"{point.Y} ({point.Participation:P0})",
            });

            StatisticsChart.Add(new PieSeries
            {
                Title = "NotAccepted Requests",
                Values = new LiveCharts.ChartValues<double> { notAcceptedPercentage },
                DataLabels = true,
                Fill = new SolidColorBrush(Colors.DarkRed),
                LabelPoint = point => $"{point.Y} ({point.Participation:P0})",
            });
        }

        public void ChartForAllYears(){
            if (SelectedYear.Equals("All years"))
            {
                AcceptedRequests.Clear();
                foreach (TourRequest request in _tourRequestService.GetAcceptedRequests())
                {
                    AcceptedRequests.Add(request);
                }
                CreateChart();
            }
        }

        public void ChartForSelectedYears()
        {
            if (!SelectedYear.Equals("All years"))
            {
                RequestsByYear.Clear();
                foreach (TourRequest request in _tourRequestService.GetRequestsByYear(SelectedYear))
                {
                    RequestsByYear.Add(request);
                }
                CreateChart();
            }

        }

        public void ChartForSelectedYear() {
            if (!SelectedYear.Equals("All years"))
            {
                AcceptedRequests.Clear();
                foreach (TourRequest request in _tourRequestService.GetAcceptedRequests())
                {
                    if (SelectedYear.Equals(request.FromDate.Value.Year.ToString()) && !ExistsRequest(request))
                    {
                        AcceptedRequests.Add(request);
                  
                    }
                }
                ChartForSelectedYears();
            }
            else {
                ChartForAllYears();
            }
        }

        public bool ExistsRequest(TourRequest request) {
            for (int i = AcceptedRequests.Count - 1; i >= 0; i--) {
                if (AcceptedRequests[i].Id == request.Id) { 
                return true;
                }
            }
            return false;
        }

        public int CountLanguage(String language) {
            int id=0;
            foreach (TourRequest request in _tourRequestService.GetAll()) {
                if (language.Equals(request.Language)) {
                    id++;
                }
            }
            return id;
        }


        public void PopulateCounts() {
            Counts.Clear();
            foreach (String language in Languages) {
                Counts.Add(CountLanguage(language));
            }
        }
        public int CountLocation(Location location)
        {
            int id = 0;
            foreach (TourRequest request in _tourRequestService.GetAll())
            {
                if (location.Id== request.Location.Id)
                {
                    id++;
                }
            }
            return id;
        }

        public void PopulateCountsLocation()
        {
            CountsLocation.Clear();
            foreach (Location location in Locations)
            {
                CountsLocation.Add(CountLocation(location));
            }
        }
        private SeriesCollection GenerateChartData(Dictionary<string, int> dataCounts)
        {
            SeriesCollection series = new SeriesCollection();

            var labels = dataCounts.Keys.ToArray();
            var values = dataCounts.Values.ToArray();

            var columnSeries = new ColumnSeries
            {
                Values = new ChartValues<int>(values),
                DataLabels = true,
                Fill = new SolidColorBrush(Colors.Green),
            };

            Axis axis = new Axis
            {
                Labels = labels
            };

            series.Add(columnSeries);

            return series;
        }

        public void PopulateDictionaryLanguage() {
            dataCounts.Clear();
            foreach (String language in Languages)
            {
                foreach (int count in Counts)
                {
                    if (!dataCounts.ContainsKey(language)) {
                        dataCounts.Add(language, count);
                    }
                }
            }
        }
        public void PopulateDictionaryLocation()
        {
            dataCounts2.Clear();
            foreach (Location location in Locations)
            {
                foreach (int count in CountsLocation)
                {
                    if (!dataCounts2.ContainsKey(location.Id.ToString()))
                    {
                        dataCounts2.Add(location.Id.ToString(), count);
                    }
                }
            }
        }
        public int SumGuestNum(List<TourRequest> Requests) {
            int sum = 0;
            foreach (TourRequest request in Requests) {
                sum += request.GuestNumber;
            }
            return sum;
        }
     
    }
}
