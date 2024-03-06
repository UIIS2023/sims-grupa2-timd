using SimsProject.Domain.Model;
using System.Collections.Generic;

namespace SimsProject.Application.Interface
{
    public interface ITourRequestService : IService<TourRequest>
    {
        List<TourRequest> GetByParentId(int parentId);
        List<TourRequest> Search(string language, string fromDate, string toDate, string city, string country, string guestNumber);
        TourRequest UpdateById(int tourRequestId);
        List<string> FindDistinctYears();
        List<string> FindDistinctCountries();
        List<string> FindDistinctCitiesByCountry(string country);
        List<TourRequest> GetAll();
        List<string> FindDistinctLanguages();
        void UpdateStatus(TourRequest request); 
        bool IsPeriodExpired(TourRequest request);
        void CheckDateRange(TourRequest request);
        List<TourRequest> GetAcceptedRequests();
        List<TourRequest> GetRequestsByYear(string year);
        bool EverAcceptedLocation(Location location);
        bool EverAcceptedLanguage(string language);
        bool ExistsLocation(List<Location> locations, Location loc);
        List<TourRequest> GetRequestsByComplexId(ComplexTourRequest complex);
        bool IsAllAccepted(List<TourRequest> requests);
    }
}
