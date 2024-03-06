using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SimsProject.WPF.ViewModel.Domain
{
    public class TourRequestListViewModel : ViewModelBase, IEnumerable
    {
        public List<TourRequestViewModel> TourRequests { get; set; }
        private readonly ITourRequestService _tourRequestService;

        public TourRequestListViewModel(ComplexTourRequest complexTourRequest)
        {
            _tourRequestService = Injector.CreateInstance<ITourRequestService>();
            TourRequests = new List<TourRequestViewModel>();

            foreach (var tourRequest in _tourRequestService.GetByParentId(complexTourRequest.Id))
            {
                TourRequests.Add(new TourRequestViewModel(tourRequest));
            }
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
