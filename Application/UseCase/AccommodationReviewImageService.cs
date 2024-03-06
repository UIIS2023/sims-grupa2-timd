using System.Collections.Generic;
using SimsProject.Domain.RepositoryInterface;
using SimsProject.Domain.Model;
using SimsProject.Application.Interface;
using System.Linq;

namespace SimsProject.Application.UseCase
{
    public class AccommodationReviewImageService : IAccommodationReviewImageService
    {
        private readonly IAccommodationReviewImageRepository _imageRepository;

        public AccommodationReviewImageService()
        {
            _imageRepository = Injector.CreateInstance<IAccommodationReviewImageRepository>();
        }

        public Image GetById(int id)
        {
            return _imageRepository.GetById(id);
        }

        public void SaveByReview(int id, IEnumerable<Image> images)
        {
            _imageRepository.SaveAllByParentId(id, images.ToList());
        }
    }
}
