using SimsProject.Serializer;
using System;
using System.Collections.Generic;

namespace SimsProject.Domain.Model
{
    public enum ComplexRequestStatus
    {
        Pending,
        Invalid,
        Accepted
    }
    public class ComplexTourRequest : ISerializable
    {
        public int Id { get; set; }
        public List<TourRequest> TourRequests { get; set; }
        public ComplexRequestStatus Status { get; set; }

        public ComplexTourRequest() 
        {
            Id = -1;
        }

        public ComplexTourRequest(ComplexRequestStatus status) 
        {
            Status = status;
            TourRequests = new List<TourRequest>();
        }

        public string[] ToCsv()
        {
            string[] csvValues =
            {
                Id.ToString(),
                ((int)Status).ToString()
            };
            return csvValues;
        }

        public void FromCsv(string[] values)
        {
            Id = int.Parse(values[0]);
            Status = (ComplexRequestStatus)Convert.ToInt32(values[1]);
        }
    }
}
