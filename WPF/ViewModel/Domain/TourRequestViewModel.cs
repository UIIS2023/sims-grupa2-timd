using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using System;

namespace SimsProject.WPF.ViewModel.Domain
{
    public class TourRequestViewModel : ViewModelBase
    {
        private readonly ITourRequestService _tourRequestService;
        private readonly TourRequest _tourRequest;

        internal int Id => _tourRequest.Id;
        public string Description => _tourRequest.Description;
        public string Language => _tourRequest.Language;
        public int GuestNumber => _tourRequest.GuestNumber;
        public DateTime? FromDate => _tourRequest.FromDate;
        public DateTime? ToDate => _tourRequest.ToDate;
        public TourRequestStatus RequestStatus
        {
            get => _tourRequest.RequestStatus;
            set => _tourRequest.RequestStatus = value;
        }
        public LocationViewModel Location { get; set; }

        public TourRequestViewModel(TourRequest tourRequest)
        {
            _tourRequestService = Injector.CreateInstance<ITourRequestService>();

            _tourRequest = _tourRequestService.GetById(tourRequest.Id);

            Location = new LocationViewModel(_tourRequest.Location);
        }
    }
}
