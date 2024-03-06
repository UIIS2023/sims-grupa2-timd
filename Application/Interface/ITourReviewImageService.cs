using SimsProject.Domain.Model;
using System.Collections.Generic;

namespace SimsProject.Application.Interface
{
    public interface ITourReviewImageService : IImageService
    {
        Image Save(Image image);
        List<Image> SaveAllByParentId(int parentId, List<Image> images);
        List<Image> GetAll();
        List<Image> GetByParentId(int parentId);
        void DeleteAllByParentId(int parentId);
    }
}
