using SimsProject.Serializer;
using System;

namespace SimsProject.Domain.Model
{
    public class TourReservation : ISerializable
    {
        public int Id { get; set; }
        public Tour Tour { get; set; }
        public User Guide { get; set; }
        public User Guest { get; set; }
        public int GuestNumber { get; set; }
        public DateTime Date { get; set; }
        public int GuestAge { get; set; }
        public bool IsVoucherUsed { get; set; }
        public TourReservation() { }

        public TourReservation(Tour tour, User guide, User guest, int guestNumber, DateTime date, int age, bool voucherUsed)
        {
            Tour = tour;
            Guest = guest;
            GuestNumber = guestNumber;
            Date = date;
            GuestAge = age;
            Guide = guide;
            IsVoucherUsed = voucherUsed;
        }

        public void FromCsv(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            Tour = new Tour() { Id = Convert.ToInt32(values[1]) };
            Guest = new User() { Id = Convert.ToInt32(values[2]) };
            GuestNumber = Convert.ToInt32(values[3]);
            GuestAge = Convert.ToInt32(values[4]);
            Date = DateTime.Parse(values[5]);
            Guide = new User() { Id = Convert.ToInt32(values[6]) };
            IsVoucherUsed = bool.Parse(values[7]);
        }

        public string[] ToCsv()
        {
            string[] csvValues = {  Id.ToString(),
                                    Tour.Id.ToString(),
                                    Guest.Id.ToString(),
                                    GuestNumber.ToString(),
                                    GuestAge.ToString(),
                                    Date.ToString(),
                                    Guide.Id.ToString(),
                                    IsVoucherUsed.ToString()
            };
            return csvValues;
        }
    }
}
