using SimsProject.Application.Interface;
using SimsProject.WPF.ViewModel.Domain;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using SimsProject.Domain.Model;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using System.IO;
using System.Linq;
using System.Reflection;
using MigraDoc.DocumentObjectModel.Tables;
using SimsProject.WPF.View;

namespace SimsProject.WPF.ViewModel.Guest1
{
    public class GenerateWindowViewModel : ViewModelBase, IDataErrorInfo
    {
        public User CurrentUser;
        private readonly Window _window;
        private readonly IAccommodationReservationService _accommodationReservationService;
        public GenerateWindowViewModel(Window window, User currentUser)
        {
            CurrentUser = currentUser;
            _window = window;
            _accommodationReservationService = Injector.CreateInstance<IAccommodationReservationService>();
            RequestCommand = new RelayCommand(RequestClick, CanRequestClick);
            GoBackCommand = new RelayCommand(GoBackClick);
            Reservations = true;
            Canceled = false;
        }
        public ICommand GoBackCommand { get; set; }
        public void GoBackClick(object obj)
        {
            _window.Close();
        }
        public ICommand RequestCommand { get; set; }
        public bool CanRequestClick(object obj)
        {
            return Date1 is not null && Date2 is not null && Date1 <= Date2 && Date2 <= DateTime.Today;
        }
        public void RequestClick(object obj)
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
            _window.Close();
        }
        private DateTime? _date1;
        public DateTime? Date1
        {
            get => _date1;
            set
            {
                if (value != _date1)
                {
                    _date1 = value;
                    OnPropertyChanged(nameof(Date1));
                    OnPropertyChanged(nameof(Date2));
                }
            }
        }
        private DateTime? _date2;
        public DateTime? Date2
        {
            get => _date2;
            set
            {
                _date2 = value;
                OnPropertyChanged(nameof(Date1));
                OnPropertyChanged(nameof(Date2));
            }
        }

        private bool _reservations;
        public bool Reservations
        {
            get => _reservations;
            set
            {
                _reservations = value;
                OnPropertyChanged(nameof(Reservations));
            }
        }

        private bool _canceled;
        public bool Canceled
        {
            get => _canceled;
            set
            {
                _canceled = value;
                OnPropertyChanged(nameof(Canceled));
            }
        }

        public string Error => null;
        public string this[string columnName]
        {
            get
            {
                string error = null;

                switch (columnName)
                {
                    case nameof(Date1):
                        if (Date1 is null)
                        {
                            error = "Date required";
                        }
                        if (Date2 < Date1)
                        {
                            error = "Dates not in order";
                        }
                        break;

                    case nameof(Date2):
                        if (Date2 is null)
                        {
                            error = "Date required";
                        }
                        if (Date2 < Date1)
                        {
                            error = "Dates not in order";
                        }
                        break;
                }
                return error;
            }
        }

        private void GeneratePdf(Document document)
        {
            var startDate = DateOnly.FromDateTime(Date1 ?? DateTime.MinValue);
            var endDate = DateOnly.FromDateTime(Date2?? DateTime.MinValue);

            var section = GeneratePdfHeader(document);
            GeneratePdfBody(startDate, endDate, section);
        }

        private void GeneratePdfBody(DateOnly startDate, DateOnly endDate, Section section)
        {
            var descriptionText = Canceled ? $"This report provides an overview of all your canceled bookings within the time period from {startDate.ToString("dd.MM.yyyy.")} to {endDate.ToString("dd.MM.yyyy.")} " +
                                  "It includes information on the accommodation, arrival date, checkout date, and number of guests.":
                $"This report provides an overview of all your scheduled bookings within the time period from {startDate.ToString("dd.MM.yyyy.")} to {endDate.ToString("dd.MM.yyyy.")} " +
                "It includes information on the accommodation, arrival date, checkout date, and number of guests.";

            var descriptionParagraph = section.AddParagraph(descriptionText);
            descriptionParagraph.Format.Font.Size = 12;

            section.AddParagraph().AddLineBreak();
            GeneratePdfTable(startDate, endDate, section);
        }

        private Section GeneratePdfHeader(Document document)
        {
            var section = document.AddSection();

            var header = section.Headers.Primary;

            var currentDateParagraph = header.AddParagraph(DateTime.Now.ToString("dd.MM.yyyy."));
            currentDateParagraph.Format.Alignment = ParagraphAlignment.Left;

            var guestParagraph = header.AddParagraph("Generated by: " + CurrentUser.Username);
            guestParagraph.Format.Alignment = ParagraphAlignment.Left;

            var titleParagraph = Canceled ? header.AddParagraph("Canceled Bookings Report") : header.AddParagraph("Scheduled Bookings Report");
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
            string[] headers = { "Accommodation", "Arrival Date", "Checkout Date", "Guest Number" };

            var columnWidths = new[] { Unit.FromCentimeter(6), Unit.FromCentimeter(3.5), Unit.FromCentimeter(3.5), Unit.FromCentimeter(3.5) };

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
            var reservations =Canceled ? _accommodationReservationService.GetByDateRangeCanceled(CurrentUser, startDate, endDate)
                .Select(reservation => new AccommodationReservationViewModel(reservation)) : _accommodationReservationService.GetByDateRange(CurrentUser, startDate, endDate)
                .Select(reservation => new AccommodationReservationViewModel(reservation));

            foreach (var reservation in reservations)
            {
                var row = table.AddRow();
                row.Cells[0].AddParagraph(reservation.Accommodation.Name);
                row.Cells[1].AddParagraph(reservation.ArrivalDate.ToString("dd.MM.yyyy."));
                row.Cells[2].AddParagraph(reservation.CheckoutDate.ToString("dd.MM.yyyy."));
                row.Cells[3].AddParagraph(reservation.GuestNumber.ToString());
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

            var dialog = new Report(temporaryFilePath);
            dialog.ShowDialog();
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
    }
}
