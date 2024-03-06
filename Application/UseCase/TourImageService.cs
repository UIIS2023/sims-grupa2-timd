using SimsProject.Domain.RepositoryInterface;
using SimsProject.Domain.Model;
using SimsProject.Application.Interface;
using System.Collections.Generic;

namespace SimsProject.Application.UseCase
{
    public class TourImageService : ITourImageService
    {
        private readonly ITourImageRepository _imageRepository;

        public TourImageService()
        {
            _imageRepository = Injector.CreateInstance<ITourImageRepository>();
        }

        public Image GetById(int id)
        {
            return _imageRepository.GetById(id);
        }

        public List<Image> GetByParentId(int parentId)
        {
            return _imageRepository.GetByParentId(parentId);
        }

        public void DeleteAllByParentId(int parentId)
        {
            _imageRepository.DeleteAllByParentId(parentId);
        }

        public List<Image> SaveAllByParentId(int parentId, List<Image> images)
        {
            return _imageRepository.SaveAllByParentId(parentId, images);
        }
    }
}
