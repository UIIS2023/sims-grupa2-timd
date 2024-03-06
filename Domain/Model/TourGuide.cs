namespace SimsProject.Domain.Model
{
    public class TourGuide : User
    {
        private const int MinLanguageCount = 20;
        private const double MinSuperGuideRating = 4.0;

        public static bool IsSuperGuide(int languageCount, double averageRating)
        {
            return languageCount >= MinLanguageCount && averageRating >= MinSuperGuideRating;
        }
    }
}
