using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using System;

namespace SimsProject.WPF.ViewModel.Domain
{
    public class AccommodationRenovationViewModel : ViewModelBase
    {
        private readonly AccommodationRenovation _renovation;

        internal int Id => _renovation.Id;
        public AccommodationViewModel Accommodation { get; }
        public User Owner { get; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int Length => _renovation.Length;
        public string Description => _renovation.Description;

        public AccommodationRenovationViewModel(AccommodationRenovation renovation)
        {
            var renovationService = Injector.CreateInstance<IAccommodationRenovationService>();
            var userService = Injector.CreateInstance<IUserService>();
            _renovation = renovationService.GetById(renovation.Id);

            Accommodation = new AccommodationViewModel(_renovation.Accommodation);
            Owner = userService.GetById(_renovation.Owner.Id);
            StartDate = _renovation.StartDate;
            EndDate = _renovation.EndDate;
        }
    }
}