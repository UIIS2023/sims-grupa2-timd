using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using SimsProject.Domain.Model;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;

namespace SimsProject.WPF.ViewModel.Guest2
{
    public class PDFGuest2ViewModel
    {
        private static string OpenFilePicker()
        {
            SaveFileDialog saveFileDialog = new();
            saveFileDialog.Filter = "PDF Files (.pdf)|.pdf";
            saveFileDialog.DefaultExt = "pdf";
            if (saveFileDialog.ShowDialog() == true)
            {
                return saveFileDialog.FileName;
            }
            throw new Exception("Save file dialog returned error!");
        }

        public static void GenerateTourReservationPDF(Tour tour, TourReservation reservation)
        {
            try
            {
                string filePath = OpenFilePicker();

                Document document = new Document();
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
                writer.SetPdfVersion(PdfWriter.PDF_VERSION_1_7);
                writer.SetFullCompression();

                document.Open();

                string assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                string imagePath = $"{assemblyName}.Resources.Images.logopdf.png";

                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(imagePath))
                {
                    if (stream != null)
                    {
                        iTextSharp.text.Image logoImage = iTextSharp.text.Image.GetInstance(stream);

                        float desiredHeight = 150f;
                        float aspectRatio = logoImage.Width / logoImage.Height;
                        float desiredWidth = desiredHeight * aspectRatio;

                        PdfPTable table = new PdfPTable(2);
                        table.DefaultCell.Border = Rectangle.NO_BORDER;

                        PdfPCell imageCell = new PdfPCell(logoImage);
                        imageCell.FixedHeight = desiredHeight;
                        imageCell.Border = Rectangle.NO_BORDER;
                        table.AddCell(imageCell);

                        PdfPCell textCell = new PdfPCell(new Phrase("Booking"));
                        textCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        textCell.Border = Rectangle.NO_BORDER;

                        BaseFont baseFont = BaseFont.CreateFont("Helvetica-Bold", "Cp1252", false);
                        Font font = new Font(baseFont, 30f);

                        Chunk chunk = new Chunk("Booking", font);
                        Phrase phrase = new Phrase(chunk);
                        textCell.AddElement(phrase);

                        table.AddCell(textCell);

                        document.Add(table);
                    }
                    else
                    {
                        MessageBox.Show("Error loading image!");
                    }
                }

                Paragraph heading = new Paragraph($"Tour reservation report: \n\n", new Font(Font.FontFamily.HELVETICA, 20, Font.BOLD))
                {
                    SpacingAfter = 15f
                };
                document.Add(heading);

                Paragraph details = new($"Your tour through {tour.TourLocation.City}, {tour.TourLocation.Country} starts at {reservation.Date:dd.MM.yyyy hh:mm:ss}, \n" +
                                        $"Number of guests you reserved for: {reservation.GuestNumber}",
                                        new Font(Font.FontFamily.HELVETICA, 16, Font.BOLD))
                {
                    SpacingAfter = 15f
                };
                document.Add(details);
                Paragraph footer = new($"Thank you for your trust \n" +
                                        $"Your Booking", new Font(Font.FontFamily.HELVETICA, 16, Font.BOLD))
                {
                    SpacingAfter = 15f
                };
                document.Add(footer);
                document.Close();


                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd",
                    Arguments = $"/c start \"\" \"{filePath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating PDF file: " + ex.Message);
            }

            
        }

       
    }
}
