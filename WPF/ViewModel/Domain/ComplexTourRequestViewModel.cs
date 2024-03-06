using SimsProject.Application.Interface;
using SimsProject.Domain.Model;

namespace SimsProject.WPF.ViewModel.Domain
{
    public class ComplexTourRequestViewModel : ViewModelBase
    {
        private readonly IComplexTourRequestService _complexTourRequestService;
        private readonly ComplexTourRequest _complexTourRequest;

        internal int Id => _complexTourRequest.Id;
        public ComplexRequestStatus ComplexRequestStatus
        {
            get => _complexTourRequest.Status;
            set => _complexTourRequest.Status = value;
        } 
        public TourRequestListViewModel TourRequestsViewModel { get; set; }

        public ComplexTourRequestViewModel(ComplexTourRequest complexTourRequest)
        {
            _complexTourRequestService = Injector.CreateInstance<IComplexTourRequestService>();
            _complexTourRequest = _complexTourRequestService.GetById(complexTourRequest.Id);
            TourRequestsViewModel = new TourRequestListViewModel(_complexTourRequest);
        }
    }
}
