using SimsProject.Domain.Model;

namespace SimsProject.Application.Dto
{
    public class AccommodationRecommendationDto
    {
        public Location Open { get; set; }
        public Location Close { get; set; }

        public AccommodationRecommendationDto(Location open, Location close)
        {
            Open = open;
            Close = close;
        }
    }
}
