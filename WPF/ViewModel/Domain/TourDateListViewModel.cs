using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SimsProject.WPF.ViewModel.Domain
{
    public class TourDateListViewModel : ViewModelBase, IEnumerable
    {
        public List<TourDateViewModel> TourDates { get; set; }
        private readonly ITourDateService _tourDateService;

        public TourDateListViewModel(Tour tour)
        {
            _tourDateService = Injector.CreateInstance<ITourDateService>();
            TourDates = new List<TourDateViewModel>();

            foreach (var tourDate in _tourDateService.GetByParentId(tour.Id))
            {
                TourDates.Add(new TourDateViewModel(tourDate));
            }
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
