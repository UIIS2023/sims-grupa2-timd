using SimsProject.Serializer;
using System;

namespace SimsProject.Domain.Model
{
    public enum TourRequestStatus
    {
        Pending, 
        Invalid, 
        Accepted
    }

    public class TourRequest : ISerializable
    {
        public int Id { get; set; }
        public User Guide { get; set; }
        public Location Location { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
        public int GuestNumber { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public TourRequestStatus RequestStatus { get; set; }
        public ComplexTourRequest ComplexTourRequest { get; set; }

        public TourRequest()
        {
            Id = -1;
        }

        public TourRequest(string description, User guide, string language, Location location, int guestNumber, DateTime? fromDate, DateTime? toDate, TourRequestStatus requestStatus, ComplexTourRequest complexTourRequest)
        {
            Guide = guide;
            Description = description;
            Language = language;
            Location = location;
            GuestNumber = guestNumber;
            FromDate = fromDate;
            ToDate = toDate;
            RequestStatus = requestStatus;
            ComplexTourRequest = complexTourRequest;
        }

        public string[] ToCsv()
        {
            string[] csvValues =
            {
                Id.ToString(),
                Guide.Id.ToString(),
                Description,
                Language,
                GuestNumber.ToString(),
                Location.Id.ToString(),
                FromDate.ToString(),
                ToDate.ToString(),
                ((int)RequestStatus).ToString(),
                ComplexTourRequest.Id.ToString(),
            };
            return csvValues;
        }

        public void FromCsv(string[] values)
        {
            Id = int.Parse(values[0]);
            Guide = new User() { Id = Convert.ToInt32(values[1]) };
            Description = values[2];
            Language = values[3];
            GuestNumber = Convert.ToInt32(values[4]);
            Location = new Location() { Id = Convert.ToInt32(values[5])};
            FromDate = DateTime.Parse(values[6]);
            ToDate = DateTime.Parse(values[7]);
            RequestStatus = (TourRequestStatus)Convert.ToInt32(values[8]);
            ComplexTourRequest = new ComplexTourRequest() { Id = Convert.ToInt32(values[9]) };
        }
    }
}
