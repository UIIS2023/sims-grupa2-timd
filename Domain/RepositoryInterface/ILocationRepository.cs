using SimsProject.Domain.Model;
using System.Collections.Generic;

namespace SimsProject.Domain.RepositoryInterface
{
    public interface ILocationRepository : IRepository<Location>
    {
        List<string> GetAllCountries();
        List<string> GetAllCities();
        List<string> GetAllCitiesByCountry(string country);
        Location GetByCityAndCountry(string city, string country);
    }
}
