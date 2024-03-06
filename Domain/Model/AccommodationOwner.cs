namespace SimsProject.Domain.Model
{
    public class AccommodationOwner : User
    {
        private const int MinSuperOwnerReviewCount = 5;
        private const double MinSuperOwnerRating = 9.5;

        public static bool IsSuperOwner(int reviewCount, double averageRating)
        {
            return reviewCount >= MinSuperOwnerReviewCount && averageRating >= MinSuperOwnerRating;
        }
    }
}
