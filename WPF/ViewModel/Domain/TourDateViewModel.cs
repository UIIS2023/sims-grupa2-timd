using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using System;

namespace SimsProject.WPF.ViewModel.Domain
{
    public class TourDateViewModel : ViewModelBase
    {
        private readonly ITourDateService _tourDateService;
        private readonly TourDate _tourDate;

        public int Id => _tourDate.Id;
        public DateTime Date => _tourDate.Date;
        public bool HasEnded
        {
            get => _tourDate.HasEnded;
            set => _tourDate.HasEnded = value;
        }

        public int TourId
        {
            get => _tourDate.Tour.Id;
            set => _tourDate.Tour.Id = value;
        }

        public TourDateViewModel(TourDate tourDate)
        {
            _tourDateService = Injector.CreateInstance<ITourDateService>();
            _tourDate = _tourDateService.GetById(tourDate.Id);
        }
    }
}
