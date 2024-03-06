using LiveCharts;
using LiveCharts.Wpf;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.WPF.ViewModel.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using Orientation = MigraDoc.DocumentObjectModel.Orientation;

namespace SimsProject.WPF.ViewModel.Guide
{
    public class TourStatisticsViewModel : ViewModelBase
    {
        #region PROPERTIES

        public List<TourReservationViewModel> TourReservations { get; set; }
        public List<TourVoucherViewModel> TourVouchers { get; set; }
        public List<TourViewModel> FinishedTours { get; set; }
        public List<TourDateViewModel> TourDates { get; set; }
        public List<ImageViewModel> Images { get; set; }
        public List<TourViewModel> Tours { get; set; }
        public SeriesCollection VoucherChart { get; set; }
        public SeriesCollection AgeChart { get; set; }
        public SeriesCollection VoucherPdfChart { get; set; }
        public SeriesCollection AgePdfChart { get; set; }

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

        private string _voucherPossession;
        public string VoucherPossession
        {
            get => _voucherPossession;
            set
            {
                if (value != _voucherPossession)
                {
                    _voucherPossession = value;
                    OnPropertyChanged(nameof(VoucherPossession));
                }
            }
        }

        private string _ageLabel;
        public string AgeLabel
        {
            get => _ageLabel;
            set
            {
                if (value != _ageLabel)
                {
                    _ageLabel = value;
                    OnPropertyChanged(nameof(AgeLabel));
                }
            }
        }

        private User LoggedInUser { get; }

        private int _guestsUnder18;
        private int _guestsBetween18And50;
        private int _guestsOver50;

        private int _totalGuests;
        private int _guestsWithVouchers;
        private int _guestsWithoutVouchers;

        #endregion

        #region COMMANDS
        public ICommand OpenFindTourCommand { get; set; }
        public ICommand GeneratePdfReportCommand { get; set; }
        public ICommand OpenGuideOverviewCommand { get; set; }

        #endregion

        #region SERVICES

        private ITourService _tourService;
        private ITourReservationService _tourReservationService;
        private ITourVoucherService _tourVoucherService;
        private ITourDateService _tourDateService;

        #endregion

        #region EVENTS

        public event EventHandler<string> ShowMessageBox;
        public static event EventHandler<ActionCompletedEventArgs> ActionCompleted;

        public event EventHandler<string> OpenReportRequested;

        #endregion

        public TourStatisticsViewModel(User guide)
        {
            LoggedInUser = guide;

            InitializeServices();
            InitializeCollections();
            InitializeCommands();
        }

        private void InitializeServices()
        {
            _tourService = Injector.CreateInstance<ITourService>();
            _tourReservationService = Injector.CreateInstance<ITourReservationService>();
            _tourVoucherService = Injector.CreateInstance<ITourVoucherService>();
            _tourDateService = Injector.CreateInstance<ITourDateService>();
        }

        private void InitializeCommands()
        {
            OpenFindTourCommand = new RelayCommand(OpenFindTour);
            OpenGuideOverviewCommand = new RelayCommand(OpenGuideOverview);
            GeneratePdfReportCommand = new RelayCommand(GeneratePdfReport, CanGeneratePdfReport);
        }

        private void InitializeCollections()
        {
            Tours = new List<TourViewModel>();
            FinishedTours = new List<TourViewModel>();
            TourVouchers = new List<TourVoucherViewModel>();
            TourDates = new List<TourDateViewModel>();
            VoucherChart = new SeriesCollection();
            VoucherPdfChart = new SeriesCollection();
            AgePdfChart = new SeriesCollection();
            AgeChart = new SeriesCollection();
            TourReservations = new List<TourReservationViewModel>();

            foreach (var tour in _tourService.GetByGuide(LoggedInUser))
            {
                Tours.Add(new TourViewModel(tour));
            }

            foreach (var tourReservation in _tourReservationService.GetByGuide(LoggedInUser))
            {
                TourReservations.Add(new TourReservationViewModel(tourReservation));
            }

            foreach (var voucher in _tourVoucherService.GetAll())
            {
                TourVouchers.Add(new TourVoucherViewModel(voucher));
            }

            foreach (var tourDate in _tourDateService.GetAll())
            {
                TourDates.Add(new TourDateViewModel(tourDate));
            }

            FindFinishedTours();
        }

        private void GeneratePdfReport(object obj)
        {

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            var document = new Document
            {
                DefaultPageSetup =
                {
                    PageFormat = PageFormat.A4,
                    Orientation = Orientation.Portrait
                }
            };

            GeneratePdf(document);
            ShowPdf(document);
        }

        private void GeneratePdf(Document document)
        {
            var section = GeneratePdfHeader(document);
            GeneratePdfBody(section);
        }

        private void GeneratePdfBody(Section section)
        {
            const string descriptionText = "This report provides a display of age distribution in guests that attended the tour, as well as percentage of guests who attended the tour with a voucher and the percentage of guests who attended without a voucher.";

            var descriptionParagraph = section.AddParagraph(descriptionText);
            descriptionParagraph.Format.Font.Size = 12;

            section.AddParagraph().AddLineBreak();

            const string voucherDescriptionText = "This chart displays the percentage of guests who attended the tour with a voucher and the percentage of guests who attended without a voucher.";
            const string voucherFileName = "voucherChart.png";
            ResetChartValues(VoucherPdfChart, AgePdfChart);
            GeneratePdfCharts(section, VoucherPdfChart, voucherDescriptionText, voucherFileName);

            const string ageDescriptionText = "This chart provides an age distribution of guests who attended the tour.";
            const string ageFileName = "ageChart.png";
            GeneratePdfCharts(section, AgePdfChart, ageDescriptionText, ageFileName);
        }

        private static void GeneratePdfCharts(Section section, SeriesCollection seriesCollection, string descriptionText, string fileName)
        {
            var rootFolderPath = AppDomain.CurrentDomain.BaseDirectory;
            GenerateChartDescription(section, descriptionText, seriesCollection);

            var chart = new PieChart()
            {
                DisableAnimations = true,
                Width = 600,
                Height = 200,
                Series = seriesCollection
            };

            var voucherViewBox = new Viewbox
            {
                Child = chart
            };

            voucherViewBox.Measure(chart.RenderSize);
            voucherViewBox.Arrange(new Rect(new Point(0, 0), chart.RenderSize));
            chart.Update(true, true);
            voucherViewBox.UpdateLayout();

            SaveToPng(chart, fileName);
            var voucherChartImageFilePath = Path.Combine(rootFolderPath, fileName);
            section.AddImage(voucherChartImageFilePath);
        }

        private static void GenerateChartDescription(Section section, string descriptionText, SeriesCollection seriesCollection)
        {
            var descriptionParagraph = section.AddParagraph(descriptionText);
            descriptionParagraph.Format.Font.Size = 12;

            var chartDescriptionParagraph = section.AddParagraph();
            chartDescriptionParagraph.Format.Font.Size = 12;

            foreach (var series in seriesCollection)
            {
                chartDescriptionParagraph.AddText($"{series.Title}: ");

                foreach (var dataPoint in series.Values)
                {
                    chartDescriptionParagraph.AddText($"{dataPoint} ");
                }

                chartDescriptionParagraph.AddLineBreak();
            }
        }

        private static void SaveToPng(FrameworkElement visual, string fileName)
        {
            var encoder = new PngBitmapEncoder();
            EncodeVisual(visual, fileName, encoder);
        }

        private static void EncodeVisual(FrameworkElement visual, string fileName, BitmapEncoder encoder)
        {
            var bitmap = new RenderTargetBitmap((int)visual.ActualWidth, (int)visual.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            var frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);
            using var stream = File.Create(fileName);
            encoder.Save(stream);
        }

        private Section GeneratePdfHeader(Document document)
        {
            var section = document.AddSection();

            var header = section.Headers.Primary;

            var currentDateParagraph = header.AddParagraph(DateTime.Now.ToString("dd/MM/yyyy"));
            currentDateParagraph.Format.Alignment = ParagraphAlignment.Left;

            var ownerParagraph = header.AddParagraph("Guide: " + LoggedInUser.Username);
            ownerParagraph.Format.Alignment = ParagraphAlignment.Left;

            var titleParagraph = header.AddParagraph("Tour statistics report");
            titleParagraph.Format.Alignment = ParagraphAlignment.Center;
            titleParagraph.Format.Font.Bold = true;
            titleParagraph.Format.Font.Size = 16;

            section.AddParagraph().AddLineBreak();
            section.AddParagraph().AddLineBreak();
            return section;
        }

        private void ShowPdf(Document document)
        {
            var temporaryFilePath = CreateTempFilePath();

            var renderer = new PdfDocumentRenderer
            {
                Document = document
            };
            renderer.RenderDocument();
            renderer.Save(temporaryFilePath);

            OpenReportRequested?.Invoke(this, temporaryFilePath);
        }

        private static string CreateTempFilePath()
        {
            const string filename = "report.pdf";

            var debugFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var temporaryFolderPath = Path.Combine(debugFolderPath ?? throw new InvalidOperationException(), "Temp");
            Directory.CreateDirectory(temporaryFolderPath);

            var uniqueFilename = $"{DateTime.Now:yyyyMMddHHmmssfff}_{filename}";
            var temporaryFilePath = Path.Combine(temporaryFolderPath, uniqueFilename);
            return temporaryFilePath;
        }

        private bool CanGeneratePdfReport(object obj)
        {
            return SelectedTour != null;
        }

        private void OpenFindTour(object obj)
        {
            if (SelectedTour == null)
            {
                VoucherPossession = " ";
                AgeLabel = " ";
                 
                const string sMessageBoxText = "Please select the tour for which you want to see statistics";
                OnShowMessageBox(sMessageBoxText);
            }
            else
            {
                VoucherPossession = "Guests with and without a voucher";
                AgeLabel = "Age statistics";

                ResetChartValues(VoucherChart, AgeChart);
            }
        } 

        private void ResetChartValues(SeriesCollection voucherChart, SeriesCollection ageChart)
        {
            voucherChart.Clear();
            ageChart.Clear();

            CountGuestsByAgeGroup(ageChart);
            CountGuestsByVouchers(voucherChart);
        }

        private void CreateVoucherChart(SeriesCollection voucherChart)
        {
            _guestsWithoutVouchers = _totalGuests - _guestsWithVouchers;

            voucherChart.Add(new PieSeries
            {
                Title = "With voucher",
                Values = new ChartValues<double> { _guestsWithVouchers },
                DataLabels = true,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F5233")!),
                LabelPoint = point => $"{point.Y} ({point.Participation:P0})"
            });
            voucherChart.Add(new PieSeries
            {
                Title = "Without voucher",
                Values = new ChartValues<double> { _guestsWithoutVouchers },
                DataLabels = true,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B1D8B7")!),
                LabelPoint = point => $"{point.Y} ({point.Participation:P0})"
            });
        }

        private void CreateAgeGroupChart(SeriesCollection ageChart)
        {
            ageChart.Add(new PieSeries
            {
                Title = "Under 18",
                Values = new ChartValues<double> { _guestsUnder18 },
                DataLabels = true,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F5233")!),
                LabelPoint = point => $"{point.Y} ({point.Participation:P0})"
            });
            ageChart.Add(new PieSeries
            {
                Title = "18-50",
                Values = new ChartValues<double> { _guestsBetween18And50 },
                DataLabels = true,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B1D8B7")!),
                LabelPoint = point => $"{point.Y} ({point.Participation:P0})"
            });
            ageChart.Add(new PieSeries
            {
                Title = "Over 50",
                Values = new ChartValues<double> { _guestsOver50 },
                DataLabels = true,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94C973")!),
                LabelPoint = point => $"{point.Y} ({point.Participation:P0})"
            });
        }

        private void CountGuestsByVouchers(SeriesCollection voucherChart)
        {
            _guestsWithVouchers = _tourVoucherService.CountGuestsWithVouchers(SelectedTour.Id);

            CreateVoucherChart(voucherChart);
        }

        private void CountGuestsByAgeGroup(SeriesCollection ageChart)
        {
            var countedAges = _tourReservationService.CountGuestsByAgeGroup(SelectedTour.Id);

            _guestsUnder18 = countedAges[0];
            _guestsBetween18And50 = countedAges[1];
            _guestsOver50 = countedAges[2];
            _totalGuests = countedAges[3];

            CreateAgeGroupChart(ageChart);
        }

        private void FindFinishedTours()
        {
            foreach (var tour in _tourService.FindFinishedTours())
            {
                FinishedTours.Add(new TourViewModel(tour));
            }
        }

        private void OpenGuideOverview(object obj)
        {
            OnActionCompleted();
        }

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
