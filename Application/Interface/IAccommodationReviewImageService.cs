using System.Collections.Generic;
using SimsProject.Domain.Model;

namespace SimsProject.Application.Interface
{
    public interface IAccommodationReviewImageService : IImageService
    {
        void SaveByReview(int id, IEnumerable<Image> images);
    }
}
