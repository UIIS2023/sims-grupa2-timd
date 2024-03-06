using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.Domain.RepositoryInterface;
using System.Collections.Generic;

namespace SimsProject.Application.UseCase
{
    public class TourReviewImageService : ITourReviewImageService
    {
        private readonly ITourReviewImageRepository _imageRepository;

        public TourReviewImageService()
        {
            _imageRepository = Injector.CreateInstance<ITourReviewImageRepository>();
        }

        public Image GetById(int id)
        {
            return _imageRepository.GetById(id);
        }

        public List<Image> GetAll()
        {
            return _imageRepository.GetAll();
        }

        public Image Save(Image image) 
        {
            return _imageRepository.Save(image);
        }

        public List<Image> SaveAllByParentId(int parentId, List<Image> images) 
        {
            return _imageRepository.SaveAllByParentId(parentId, images);
        }

        public List<Image> GetByParentId(int parentId) 
        {
            return _imageRepository.GetByParentId(parentId);
        }

        public void DeleteAllByParentId(int parentId)
        {
            _imageRepository.DeleteAllByParentId(parentId);
        }
    }
}
