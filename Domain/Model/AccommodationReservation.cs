using SimsProject.Serializer;
using System;

namespace SimsProject.Domain.Model
{
    public class AccommodationReservation : ISerializable
    {
        public int Id { get; set; }
        public Accommodation Accommodation { get; set; }
        public User Guest { get; set; }
        public User Owner { get; set; }
        public DateOnly ArrivalDate { get; set; }
        public DateOnly CheckoutDate { get; set; }
        public int StayLength { get; set; }
        public int GuestNumber { get; set; }
        public bool IsCanceled { get; set; } 
        public bool NotifyOwnerOfCancel { get; set; }

        public AccommodationReservation() { }

        public AccommodationReservation(Accommodation accommodation, User guest, User owner, DateOnly arrivalDate, int stayLength, int guestNumber)
        {
            Accommodation = accommodation;
            Guest = guest;
            Owner = owner;
            ArrivalDate = arrivalDate;
            StayLength = stayLength;
            CheckoutDate = arrivalDate.AddDays(stayLength - 1);
            GuestNumber = guestNumber;
            IsCanceled = false;
            NotifyOwnerOfCancel = false;
        }

        public void FromCsv(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            Accommodation = new Accommodation() { Id = Convert.ToInt32(values[1]) };
            Guest = new User() { Id = Convert.ToInt32(values[2]) };
            Owner = new User() { Id = Convert.ToInt32(values[3]) };
            ArrivalDate = DateOnly.Parse(values[4]);
            StayLength = Convert.ToInt32(values[5]);
            CheckoutDate = ArrivalDate.AddDays(StayLength - 1);
            GuestNumber = Convert.ToInt32(values[6]);
            IsCanceled = Convert.ToBoolean(values[7]);
            NotifyOwnerOfCancel = Convert.ToBoolean(values[8]);
        }

        public string[] ToCsv()
        {
            string[] csvValues = {  Id.ToString(),
                                    Accommodation.Id.ToString(),
                                    Guest.Id.ToString(),
                                    Owner.Id.ToString(),
                                    ArrivalDate.ToString("dd.MM.yyyy."),
                                    StayLength.ToString(),
                                    GuestNumber.ToString(),
                                    IsCanceled.ToString(),
                                    NotifyOwnerOfCancel.ToString()
                                    };
            return csvValues;
        }
    }
}
