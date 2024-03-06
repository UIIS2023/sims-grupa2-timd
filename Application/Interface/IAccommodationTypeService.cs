using System.Collections.Generic;
using SimsProject.Domain.Model;

namespace SimsProject.Application.Interface
{
    public interface IAccommodationTypeService : IService<AccommodationType>
    {
        List<AccommodationType> GetAll();
    }
}
