using SimsProject.Domain.Model;
using System.Collections.Generic;

namespace SimsProject.Application.Interface
{
    public interface ILocationService : IService<Location>
    { 
        List<string> GetDistinctCountries();
        List<Location> GetAll();
        List<string> GetDistinctCities();
        List<string> GetAllCitiesByCountry(string country);
        Location GetByCityAndCountry(string city, string country);
    }
}
