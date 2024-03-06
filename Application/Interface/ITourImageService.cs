using SimsProject.Domain.Model;
using System.Collections.Generic;

namespace SimsProject.Application.Interface
{
    public interface ITourImageService : IImageService
    {
        List<Image> SaveAllByParentId(int parentId, List<Image> images);
        List<Image> GetByParentId(int parentId);
        void DeleteAllByParentId(int parentId);
    }
}
