using System;
using SimsProject.Serializer;

namespace SimsProject.Domain.Model
{
    public class AccommodationRenovation : ISerializable
    {
        public int Id { get; set; }
        public Accommodation Accommodation { get; set; }
        public User Owner { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int Length { get; set; }
        public string Description { get; set; }

        public AccommodationRenovation() { }
        public AccommodationRenovation(Accommodation accommodation, User owner, DateOnly startDate, int length, string description)
        {
            Accommodation = accommodation;
            Owner = owner;
            StartDate = startDate;
            Length = length;
            EndDate = StartDate.AddDays(length - 1);
            Description = description;
        }
        public string[] ToCsv()
        {
            string[] csvValues = { Id.ToString(), Accommodation.Id.ToString(), Owner.Id.ToString(), StartDate.ToString("dd.MM.yyyy."), Length.ToString(), Description };
            return csvValues;
        }

        public void FromCsv(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            Accommodation = new Accommodation() { Id = Convert.ToInt32(values[1]) };
            Owner = new User() { Id = Convert.ToInt32(values[2]) };
            StartDate = DateOnly.Parse(values[3]);
            Length = Convert.ToInt32(values[4]);
            EndDate = StartDate.AddDays(Length - 1);
            Description = values[5];
        }

    }
}