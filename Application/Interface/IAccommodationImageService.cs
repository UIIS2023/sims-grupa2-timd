using System.Collections.Generic;
using SimsProject.Domain.Model;

namespace SimsProject.Application.Interface
{
    public interface IAccommodationImageService : IImageService
    {
        List<Image> GetByAccommodation(Accommodation accommodation);
        List<Image> SaveByAccommodation(int id, List<Image> images);
        void DeleteByAccommodation(int id);

    }
}
