using SimsProject.Application.DTO;

namespace SimsProject.WPF.ViewModel.Owner
{
    public class AccommodationStatisticViewModel
    {
        private readonly AccommodationStatisticDto _statistic;

        public int Year => _statistic.Year;
        public int? Month => _statistic.Month;
        public int ReservationCount => _statistic.ReservationCount;
        public int CancellationCount => _statistic.CancellationCount;
        public int RescheduleCount => _statistic.RescheduleCount;
        public int RenovationReferralCount => _statistic.RenovationReferralCount;

        public AccommodationStatisticViewModel(AccommodationStatisticDto statistic)
        {
            _statistic = statistic;
        }
    }
}
