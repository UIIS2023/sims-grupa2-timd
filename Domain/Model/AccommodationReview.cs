﻿using System;
using System.Collections.Generic;
using SimsProject.Serializer;

namespace SimsProject.Domain.Model
{
    public class AccommodationReview : ISerializable
    {
        public int Id { get; set; }
        public int CleanlinessGrade { get; set; }
        public int OwnerGrade { get; set; }
        public string Comment { get; set; }
        public User Owner { get; set; }
        public User Guest { get; set; }
        public AccommodationReservation Reservation { get; set; }
        public List<Image> Images { get; set; }
        public int UrgencyLevel { get; set; }
        public string RenovationComment { get; set; }

        public AccommodationReview() { }

        public AccommodationReview(int cleanlinessGrade, int ownerGrade, string comment, User owner, User guest, AccommodationReservation reservation, List<Image> images, int urgencyLevel, string renovationComment)
        {
            CleanlinessGrade = cleanlinessGrade;
            OwnerGrade = ownerGrade;
            Comment = comment;
            Owner = owner;
            Guest = guest;
            Reservation = reservation;
            Images = images;
            UrgencyLevel = urgencyLevel;
            RenovationComment = renovationComment;
        }

        public string[] ToCsv()
        {
            string[] csvValues = { Id.ToString(), CleanlinessGrade.ToString(), OwnerGrade.ToString(), Comment, Owner.Id.ToString(), Guest.Id.ToString(), Reservation.Id.ToString(), UrgencyLevel.ToString(), RenovationComment };
            return csvValues;
        }

        public void FromCsv(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            CleanlinessGrade = Convert.ToInt32(values[1]);
            OwnerGrade = Convert.ToInt32(values[2]);
            Comment = values[3];
            Owner = new User() { Id = Convert.ToInt32(values[4]) };
            Guest = new User() { Id = Convert.ToInt32(values[5]) };
            Reservation = new AccommodationReservation() { Id = Convert.ToInt32(values[6]) };
            UrgencyLevel = Convert.ToInt32(values[7]);
            RenovationComment = values[8];
        }
    }
}
