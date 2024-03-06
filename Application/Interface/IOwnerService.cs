using SimsProject.Application.Dto;
using SimsProject.Domain.Model;

namespace SimsProject.Application.Interface
{
    public interface IOwnerService
    {
        double GetAverageRating(User owner);
        int GetAccommodationCount(User owner);
        int GetPastReservationCount(User owner);
        int GetReviewCount(User owner);
        bool GetSuperOwnerStatus(User owner);
        AccommodationRecommendationDto GetRecommendations(User owner);
    }
}
