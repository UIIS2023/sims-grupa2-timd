using SimsProject.Domain.RepositoryInterface;
using SimsProject.Domain.Model;
using SimsProject.Application.Interface;
using System.Collections.Generic;

namespace SimsProject.Application.UseCase
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;

        public LocationService()
        {
            _locationRepository = Injector.CreateInstance<ILocationRepository>();
        }

        public Location GetById(int id)
        {
            return _locationRepository.GetById(id);
        }

        public List<Location> GetAll()
        {
            return _locationRepository.GetAll();
        }

        public List<string> GetDistinctCountries()
        {
            return _locationRepository.GetAllCountries();
        }

        public List<string> GetDistinctCities()
        {
            return _locationRepository.GetAllCities();
        }

        public List<string> GetAllCitiesByCountry(string country)
        {
            return _locationRepository.GetAllCitiesByCountry(country);
        }

        public Location GetByCityAndCountry(string city, string country)
        {
            return _locationRepository.GetByCityAndCountry(city, country);
        }
    }
}
