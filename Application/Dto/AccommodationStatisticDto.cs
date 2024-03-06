namespace SimsProject.Application.DTO
{
    public class AccommodationStatisticDto
    {
        public int AccommodationId { get; set; }
        public int Year { get; set; }
        public int? Month { get; set; }
        public int ReservationCount { get; set; }
        public int CancellationCount { get; set; }
        public int RescheduleCount { get; set; }
        public int RenovationReferralCount { get; set; }
        public double OccupancyRate { get; set; }

        public AccommodationStatisticDto(int accommodationId, int year, int? month, int reservationCount, int cancellationCount, int rescheduleCount, int renovationReferralCount)
        {
            AccommodationId = accommodationId;
            Year = year;
            Month = month;
            ReservationCount = reservationCount;
            CancellationCount = cancellationCount;
            RescheduleCount = rescheduleCount;
            RenovationReferralCount = renovationReferralCount;
            OccupancyRate = 0;
        }
    }
}