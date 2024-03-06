using System.Collections.Generic;
using SimsProject.Domain.Model;

namespace SimsProject.Application.Interface
{
    public interface IAccommodationService : IService<Accommodation>
    {
        List<Accommodation> GetAll();
        List<Accommodation> GetByOwner(User owner);
        List<Accommodation> Search(string name, string guestNumber, string stayLength, string city, string country, string type);
        Accommodation CreateAccommodation(Accommodation accommodation);
        void DeleteAccommodation(int id);
    }
}
