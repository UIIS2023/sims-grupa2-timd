using System;
using System.Collections.Generic;
using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.Domain.RepositoryInterface;

namespace SimsProject.Application.UseCase
{
    public class AccommodationService : IAccommodationService
    {
        private readonly IAccommodationRepository _accommodationRepository;
        private readonly IAccommodationTypeService _accommodationTypeService;
        private readonly ILocationService _locationService;
        private readonly IAccommodationImageService _imageService;

        public AccommodationService()
        {
            _accommodationRepository = Injector.CreateInstance<IAccommodationRepository>();
            _locationService = Injector.CreateInstance<ILocationService>();
            _accommodationTypeService = Injector.CreateInstance<IAccommodationTypeService>();
            _imageService = Injector.CreateInstance<IAccommodationImageService>();
        }

        public Accommodation GetById(int id)
        {
            return _accommodationRepository.GetById(id);
        }

        public List<Accommodation> GetAll()
        {
            return _accommodationRepository.GetAll();
        }

        public List<Accommodation> GetByOwner(User owner)
        {
            return _accommodationRepository.GetByOwner(owner);
        }

        public Accommodation CreateAccommodation(Accommodation accommodation)
        {
            accommodation.Images = _imageService.SaveByAccommodation(_accommodationRepository.NextId(), accommodation.Images);

            var savedAccommodation = _accommodationRepository.Save(accommodation);
            return savedAccommodation;
        }

        public void DeleteAccommodation(int id)
        {
            var accommodation = _accommodationRepository.GetById(id);

            _imageService.DeleteByAccommodation(accommodation.Id);
            _accommodationRepository.Delete(accommodation);
        }

        private bool IsValid(Accommodation accommodation, string name, string guestNumber, string stayLength, string city, string country, string type)
        {
            var isNameValid = string.IsNullOrEmpty(name) || accommodation.Name.ToLower().Contains(name.ToLower());
            var isGuestNumberValid = string.IsNullOrEmpty(guestNumber) || accommodation.MaxGuestNumber >= Convert.ToInt32(guestNumber);
            var isStayLengthValid = string.IsNullOrEmpty(stayLength) || accommodation.MinReservationDays <= Convert.ToInt32(stayLength);
            var isTypeValid = string.IsNullOrEmpty(type) || type == "Any" || _accommodationTypeService.GetById(accommodation.Type.Id).Name == type;
            var isCountryValid = string.IsNullOrEmpty(country) || country == "Any" || _locationService.GetById(accommodation.Location.Id).Country == country;
            var isCityValid = string.IsNullOrEmpty(city) || city == "Any" || _locationService.GetById(accommodation.Location.Id).City == city;

            return isNameValid && isGuestNumberValid && isStayLengthValid && isTypeValid && isCountryValid && isCityValid;
        }

        public List<Accommodation> Search(string name, string guestNumber, string stayLength, string city, string country, string type)
        {
            List<Accommodation> accommodations = new();

            foreach (var accommodation in _accommodationRepository.GetAll())
            {
                if (!IsValid(accommodation, name, guestNumber, stayLength, city, country, type)) continue;
                accommodations.Add(accommodation);
            }
            return accommodations;
        }
    }
}
