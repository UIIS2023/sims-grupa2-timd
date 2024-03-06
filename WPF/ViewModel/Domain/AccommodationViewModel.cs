using System;
using System.Collections.Generic;
using SimsProject.Application.Interface;
using SimsProject.Domain.Model;

namespace SimsProject.WPF.ViewModel.Domain
{
    public class AccommodationViewModel : ViewModelBase
    {
        private readonly Accommodation _accommodation;

        internal int Id => _accommodation.Id;
        public string Name { get; }
        public LocationViewModel Location { get; }
        public AccommodationTypeViewModel Type { get; }
        public int MaxGuestNumber => _accommodation.MaxGuestNumber;
        public int MinReservationDays => _accommodation.MinReservationDays;
        public int MinDaysBeforeCancellation => _accommodation.MinDaysBeforeCancellation;
        public User Owner { get; }
        public ImageViewModel Cover { get; }
        public List<ImageViewModel> Images { get; }
        public bool IsSuperUserOwned { get; }
        public bool IsRecentlyRenovated { get; }

        public AccommodationViewModel(Accommodation accommodation)
        {
            var accommodationService = Injector.CreateInstance<IAccommodationService>();
            var renovationService = Injector.CreateInstance<IAccommodationRenovationService>();
            var userService = Injector.CreateInstance<IUserService>();
            var imageService = Injector.CreateInstance<IAccommodationImageService>();

            _accommodation = accommodationService.GetById(accommodation.Id);
            var mostRecentRenovation = renovationService.GetMostRecentByAccommodation(_accommodation);

            Name = string.IsNullOrEmpty(accommodation.Name) ? _accommodation.Name : accommodation.Name;
            Location = new LocationViewModel(_accommodation.Location);
            Type = new AccommodationTypeViewModel(_accommodation.Type);
            Owner = userService.GetById(_accommodation.Owner.Id);
            Cover = new ImageViewModel(imageService, _accommodation.Cover);

            Images = new List<ImageViewModel>();
            foreach (var image in imageService.GetByAccommodation(_accommodation))
            {
                Images.Add(new ImageViewModel(imageService, image));
            }

            IsSuperUserOwned = Owner.IsSuperUser;
            IsRecentlyRenovated = mostRecentRenovation is not null
                                  && DateOnly.FromDateTime(DateTime.Now) <= mostRecentRenovation.EndDate.AddYears(1);
        }
    }
}
