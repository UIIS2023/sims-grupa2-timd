using SimsProject.Application.Interface;
using SimsProject.WPF.ViewModel.Domain;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System;
using System.ComponentModel;
using System.IO;
using SimsProject.Domain.Model;
using SimsProject.Application.Dto;
using System.Reflection;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using MigraDoc.DocumentObjectModel.Tables;
using Orientation = MigraDoc.DocumentObjectModel.Orientation;

namespace SimsProject.WPF.ViewModel.Owner
{
    public class RenovationsPanelViewModel : ViewModelBase, IDataErrorInfo
    {
        public User Owner { get; set; }
        public ObservableCollection<AccommodationRenovationViewModel> UpcomingRenovations { get; set; }
        public ObservableCollection<AccommodationRenovationViewModel> PastRenovations { get; set; }
        public ObservableCollection<Accommodation> Accommodations { get; set; }
        public ObservableCollection<DateRangeDto> AvailableDateRanges { get; set; }

        private const int MinCancellationNotice = 5;

        #region SERVICES

        private IAccommodationRenovationService _renovationService;
        private IAccommodationReservationService _reservationService;
        private IAccommodationService _accommodationService;


        #endregion

        #region PROPERTIES

        public DateTime? ReportStartDate;
        public DateTime? ReportEndDate;

        private AccommodationRenovationViewModel _selectedRenovation;
        public AccommodationRenovationViewModel SelectedRenovation
        {
            get => _selectedRenovation;
            set
            {
                if (value != _selectedRenovation)
                {
                    _selectedRenovation = value;
                    OnPropertyChanged(nameof(SelectedRenovation));
                }
            }
        }

        private Accommodation _selectedAccommodation;
        public Accommodation SelectedAccommodation
        {
            get => _selectedAccommodation;
            set
            {
                if (value != _selectedAccommodation)
                {
                    _selectedAccommodation = value;
                    OnPropertyChanged(nameof(SelectedAccommodation));
                }
            }
        }

        private DateRangeDto _selectedDateRange;
        public DateRangeDto SelectedDateRange
        {
            get => _selectedDateRange;
            set
            {
                if (value != _selectedDateRange)
                {
                    _selectedDateRange = value;
                    OnPropertyChanged(nameof(SelectedDateRange));
                }
            }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                if (value != _description)
                {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        private DateTime? _dateRangeStart;
        public DateTime? DateRangeStart
        {
            get => _dateRangeStart;
            set
            {
                if (value != _dateRangeStart)
                {
                    _dateRangeStart = value;
                    OnPropertyChanged(nameof(DateRangeStart));
                    OnPropertyChanged(nameof(DateRangeEnd));
                    OnPropertyChanged(nameof(Length));
                }
            }
        }

        private DateTime? _dateRangeEnd;
        public DateTime? DateRangeEnd
        {
            get => _dateRangeEnd;
            set
            {
                if (value != _dateRangeEnd)
                {
                    _dateRangeEnd = value;
                    OnPropertyChanged(nameof(DateRangeStart));
                    OnPropertyChanged(nameof(DateRangeEnd));
                    OnPropertyChanged(nameof(Length));
                }
            }
        }

        private int _length;
        public int Length
        {
            get => _length;
            set
            {
                if (value != _length)
                {
                    _length = value;
                    OnPropertyChanged(nameof(DateRangeStart));
                    OnPropertyChanged(nameof(DateRangeEnd));
                    OnPropertyChanged(nameof(Length));
                }
            }
        }

        #endregion

        #region COMMANDS

        public ICommand CancelCommand { get; set; }
        public ICommand GenerateReportCommand { get; set; }
        public ICommand FindDatesCommand { get; set; }
        public ICommand ScheduleCommand { get; set; }

        #endregion
        
        #region VALIDATION

        public string this[string columnName]
        {
            get
            {
                string error = null;

                switch (columnName)
                {
                    case nameof(Description):
                        if (string.IsNullOrEmpty(Description))
                        {
                            error = "Description is required";
                        }
                        break;

                    case nameof(DateRangeStart):
                        if (DateRangeStart is null)
                        {
                            error = "Start date is required";
                        }
                        if (DateRangeStart.HasValue && DateRangeEnd.HasValue && DateRangeEnd.Value < DateRangeStart.Value)
                        {
                            error = "End date must be after start date.";
                        }
                        break;

                    case nameof(DateRangeEnd):
                        if (DateRangeEnd is null)
                        {
                            error = "End date is required";
                        }
                        if (DateRangeStart.HasValue && DateRangeEnd.HasValue && DateRangeEnd.Value < DateRangeStart.Value)
                        {
                            error = "End date must be after start date.";
                        }
                        break;

                    case nameof(Length):
                        if (DateRangeStart.HasValue && DateRangeEnd.HasValue && DateRangeStart.HasValue)
                        {
                            if (!string.IsNullOrEmpty(this["StartDate"]) || !string.IsNullOrEmpty(this["EndDate"])) break;

                            var timeDifference = DateRangeEnd.Value - DateRangeStart.Value;
                            if (timeDifference.TotalDays + 1 < Length)
                            {
                                error = "The expected length of the renovation cannot be longer than the selected date range.";
                            }
                        }
                        break;
                }
                return error;
            }
        }

        public string Error => null;

        private readonly string[] _canFindDatesValidatedProperties = { "StartDate", "EndDate", "Length" };

        #endregion

        #region EVENTS

        public bool ConfirmActionResult { get; set; }
        public event EventHandler ConfirmActionRequested;

        public bool GenerateReportResult { get; set; }
        public event EventHandler GenerateReportRequested;

        public event EventHandler<string> OpenReportRequested;

        #endregion

        public RenovationsPanelViewModel(User owner)
        {
            Owner = owner;

            InitializeServices();
            InitializeCollections();
            InitializeCommands();
            SetDefaultValues();
        }

        private void InitializeServices()
        {
            _renovationService = Injector.CreateInstance<IAccommodationRenovationService>();
            _reservationService = Injector.CreateInstance<IAccommodationReservationService>();
            _accommodationService = Injector.CreateInstance<IAccommodationService>();
        }

        private void InitializeCollections()
        {
            var upcomingRenovations = (from renovation in _renovationService.GetUpcomingByOwner(Owner)
                                       select new AccommodationRenovationViewModel(renovation)).ToList();
            UpcomingRenovations = new ObservableCollection<AccommodationRenovationViewModel>(upcomingRenovations.OrderBy(r => r.StartDate).ToList());

            var pastRenovations = (from renovation in _renovationService.GetPastByOwner(Owner)
                                   select new AccommodationRenovationViewModel(renovation)).ToList();
            PastRenovations = new ObservableCollection<AccommodationRenovationViewModel>(pastRenovations.OrderByDescending(r => r.StartDate).ToList());

            Accommodations = new ObservableCollection<Accommodation>();
            foreach (var accommodation in _accommodationService.GetByOwner(Owner))
            {
                Accommodations.Add(accommodation);
            }

            AvailableDateRanges = new ObservableCollection<DateRangeDto>();
        }

        private void InitializeCommands()
        {
            CancelCommand = new RelayCommand(Cancel, CanCancel);
            GenerateReportCommand = new RelayCommand(GenerateReport);
            FindDatesCommand = new RelayCommand(FindDates, CanFindDates);
            ScheduleCommand = new RelayCommand(Schedule, CanSchedule);
        }

        private void SetDefaultValues()
        {
            SelectedAccommodation = Accommodations.Any() ? Accommodations[0] : null;
            Description = null;
            DateRangeStart = null;
            DateRangeEnd = null;
            Length = 1;
            AvailableDateRanges.Clear();
        }

        private void Cancel(object obj)
        {
            ConfirmActionRequested?.Invoke(this, EventArgs.Empty);
            if (!ConfirmActionResult) return;
            
            _renovationService.CancelRenovation(SelectedRenovation.Id);
            UpcomingRenovations.Remove(SelectedRenovation);
        }

        private void GenerateReport(object obj)
        {
            GenerateReportRequested?.Invoke(this, EventArgs.Empty);
            if (!GenerateReportResult) return;

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
            var startDate = DateOnly.FromDateTime(ReportStartDate ?? DateTime.MinValue);
            var endDate = DateOnly.FromDateTime(ReportEndDate ?? DateTime.MinValue);

            var section = GeneratePdfHeader(document);
            GeneratePdfBody(startDate, endDate, section);
        }

        private void GeneratePdfBody(DateOnly startDate, DateOnly endDate, Section section)
        {
            var descriptionText = $"This report provides an overview of all scheduled renovations for all of your accommodations within the time period from {startDate.ToString("dd.MM.yyyy")} to {endDate.ToString("dd.MM.yyyy")}. " +
                                  "It includes information on the accommodation, start dates, end dates, and descriptions of the renovations.";

            var descriptionParagraph = section.AddParagraph(descriptionText);
            descriptionParagraph.Format.Font.Size = 12;

            section.AddParagraph().AddLineBreak();
            GeneratePdfTable(startDate, endDate, section);
        }

        private Section GeneratePdfHeader(Document document)
        {
            var section = document.AddSection();

            var header = section.Headers.Primary;

            var currentDateParagraph = header.AddParagraph(DateTime.Now.ToString("dd.MM.yyyy"));
            currentDateParagraph.Format.Alignment = ParagraphAlignment.Left;

            var ownerParagraph = header.AddParagraph("Owner: " + Owner.Username);
            ownerParagraph.Format.Alignment = ParagraphAlignment.Left;

            var titleParagraph = header.AddParagraph("Renovation Report");
            titleParagraph.Format.Alignment = ParagraphAlignment.Center;
            titleParagraph.Format.Font.Bold = true;
            titleParagraph.Format.Font.Size = 16;

            section.AddParagraph().AddLineBreak();
            section.AddParagraph().AddLineBreak();
            return section;
        }

        private void GeneratePdfTable(DateOnly startDate, DateOnly endDate, Section section)
        {
            var table = section.AddTable();
            table.Borders.Width = 0.75;

            AddTableHeader(table);
            AddTableRows(startDate, endDate, table);
        }

        private void AddTableHeader(Table table)
        {
            string[] headers = { "Accommodation", "Start Date", "End Date", "Description" };

            var columnWidths = new[] { Unit.FromCentimeter(6), Unit.FromCentimeter(2.5), Unit.FromCentimeter(2.5), Unit.FromCentimeter(6) };

            foreach (var width in columnWidths)
            {
                table.AddColumn(width);
            }

            var headerRow = table.AddRow();
            headerRow.HeadingFormat = true;
            headerRow.Format.Font.Bold = true;

            for (var i = 0; i < headers.Length; i++)
            {
                var headerCell = headerRow.Cells[i];
                headerCell.AddParagraph(headers[i]);
            }
        }

        private void AddTableRows(DateOnly startDate, DateOnly endDate, Table table)
        {
            var renovations = _renovationService.GetByDateRange(startDate, endDate)
                .Select(renovation => new AccommodationRenovationViewModel(renovation));

            foreach (var renovationViewModel in renovations)
            {
                var row = table.AddRow();
                row.Cells[0].AddParagraph(renovationViewModel.Accommodation.Name);
                row.Cells[1].AddParagraph(renovationViewModel.StartDate.ToString("dd.MM.yyyy."));
                row.Cells[2].AddParagraph(renovationViewModel.EndDate.ToString("dd.MM.yyyy."));
                row.Cells[3].AddParagraph(renovationViewModel.Description);
            }
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
            var filename = "report.pdf";

            var debugFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var temporaryFolderPath = Path.Combine(debugFolderPath ?? throw new InvalidOperationException(), "Temp");
            Directory.CreateDirectory(temporaryFolderPath);

            var uniqueFilename = $"{DateTime.Now:yyyyMMddHHmmssfff}_{filename}";
            var temporaryFilePath = Path.Combine(temporaryFolderPath, uniqueFilename);
            return temporaryFilePath;
        }

        private bool CanCancel(object obj)
        {
            if (!UpcomingRenovations.Any()) return false;

            if (obj is not AccommodationRenovationViewModel renovation) return false;

            var lastDateToCancel = renovation.StartDate.AddDays(-MinCancellationNotice);
            var isNoticeExpired = DateOnly.FromDateTime(DateTime.Today) > lastDateToCancel;
            return !isNoticeExpired;
        }

        private void FindDates(object obj)
        {
            AvailableDateRanges.Clear();

            var startDate = DateOnly.FromDateTime(DateRangeStart ?? DateTime.MinValue);
            var endDate = DateOnly.FromDateTime(DateRangeEnd ?? DateTime.MinValue);

            var availableDateRanges = _reservationService.FindAvailableDates(startDate, endDate, Length, SelectedAccommodation);
            foreach (var dateRange in availableDateRanges)
            {
                AvailableDateRanges.Add(dateRange);
            }
        }

        private bool CanFindDates(object obj)
        {
            return _canFindDatesValidatedProperties.All(property => string.IsNullOrEmpty(this[property]));
        }

        private void Schedule(object obj)
        {
            var newRenovation = new AccommodationRenovation(SelectedAccommodation, Owner, SelectedDateRange.StartDate, Length, Description);
            var savedRenovation = _renovationService.ScheduleRenovation(newRenovation);
            UpcomingRenovations.Add(new AccommodationRenovationViewModel(savedRenovation));

            SetDefaultValues();
        }

        private bool CanSchedule(object obj)
        {
            return string.IsNullOrEmpty(this["Description"]) && SelectedDateRange is not null;
        }
    }
}