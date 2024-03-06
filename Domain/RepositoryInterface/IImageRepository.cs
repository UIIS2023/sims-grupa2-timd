using System.Collections.Generic;
using SimsProject.Domain.Model;

namespace SimsProject.Domain.RepositoryInterface
{
    public interface IImageRepository : IRepository<Image>
    {
        List<Image> GetByParentId(int parentId);
        List<Image> SaveAllByParentId(int parentId, List<Image> images);
        void DeleteAllByParentId(int parentId);
    }
}
