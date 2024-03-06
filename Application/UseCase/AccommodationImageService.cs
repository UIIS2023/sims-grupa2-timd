using SimsProject.Domain.RepositoryInterface;
using SimsProject.Domain.Model;
using SimsProject.Application.Interface;
using System.Collections.Generic;

namespace SimsProject.Application.UseCase
{
    public class AccommodationImageService : IAccommodationImageService
    {
        private readonly IAccommodationImageRepository _imageRepository;

        public AccommodationImageService()
        {
            _imageRepository = Injector.CreateInstance<IAccommodationImageRepository>();
        }

        public Image GetById(int id)
        {
            return _imageRepository.GetById(id);
        }

        public List<Image> GetByAccommodation(Accommodation accommodation)
        {
            return _imageRepository.GetByParentId(accommodation.Id);
        }

        public List<Image> SaveByAccommodation(int id, List<Image> images)
        {
            return _imageRepository.SaveAllByParentId(id, images);
        }

        public void DeleteByAccommodation(int id)
        { 
            _imageRepository.DeleteAllByParentId(id);
        }
    }
}
