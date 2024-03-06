using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.Domain.RepositoryInterface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SimsProject.Application.UseCase
{
    public class TourRequestService : ITourRequestService
    {
        private readonly ITourRequestRepository _tourRequestRepository;
        private readonly ILocationService _locationService;

        public TourRequestService()
        {
            _tourRequestRepository = Injector.CreateInstance<ITourRequestRepository>();
            _locationService = Injector.CreateInstance<ILocationService>();
        }

        public TourRequest GetById(int id)
        {
            return _tourRequestRepository.GetById(id);
        }

        public List<TourRequest> GetAll()
        {
            return _tourRequestRepository.GetAll();
        }

        public List<TourRequest> GetByParentId(int parentId)
        {
            return _tourRequestRepository.GetByParentId(parentId);
        }

        public List<TourRequest> Search(string language, string fromDate, string toDate, string city, string country, string guestNumber)
        {
            return _tourRequestRepository.GetAll().Where(tourRequest => IsValid(tourRequest, language, fromDate, toDate, city, country, guestNumber) && tourRequest.RequestStatus == TourRequestStatus.Pending).ToList();
        }

        public TourRequest UpdateById(int tourRequestId)
        {
            return _tourRequestRepository.UpdateById(tourRequestId);
        }

        public List<string> FindDistinctCitiesByCountry(string country)
        {
            var tourRequests = GetTourRequests();
            var cities = new List<string>();

            foreach (var request in tourRequests.Where(request => request.Location.Country == country && !cities.Contains(request.Location.City)))
            {
                cities.Add(request.Location.City);
            }

            return cities;
        }

        public List<string> FindDistinctCountries()
        {
            var tourRequests = GetTourRequests();

            var countries = new List<string>();
            foreach (var request in tourRequests
                         .Where(request => !countries.Contains(request.Location.Country)))
            {
                countries.Add(request.Location.Country);
            }

            return countries;
        }

        private IEnumerable<TourRequest> GetTourRequests()
        {
            var tourRequests = _tourRequestRepository.GetAll();
            foreach (var request in tourRequests)
            {
                request.Location = _locationService.GetById(request.Location.Id);
            }

            return tourRequests;
        }

        public List<string> FindDistinctYears()
        {
            var years = new List<string>();

            foreach (var request in _tourRequestRepository.GetAll().Where(request => request.FromDate != null && !years.Contains(request.FromDate.Value.Year.ToString())))
            {
                if (request.FromDate != null) years.Add(request.FromDate.Value.Year.ToString());
            }
            return years;
        }

        public List<string> FindDistinctLanguages()
        {
            var languages = new List<string>();

            foreach (var request in _tourRequestRepository.GetAll().Where(request => !languages.Contains(request.Language)))
            {
                languages.Add(request.Language);
            }

            return languages;
        }

        private bool IsValid(TourRequest tourRequest, string language, string fromDate, string toDate, string city, string country, string guestNumber)
        {
            DateTime.TryParseExact(toDate, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedToDate);
            DateTime.TryParseExact(fromDate, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedFromDate);

            var isDateValid = parsedFromDate <= tourRequest.FromDate || parsedToDate >= tourRequest.ToDate || parsedFromDate.Equals(DateTime.MinValue) || parsedFromDate.Equals(DateTime.MinValue);
            var isCountryValid = string.IsNullOrEmpty(country) || country == "Any" || _locationService.GetById(tourRequest.Location.Id).Country == country;
            var isCityValid = string.IsNullOrEmpty(city) || city == "Any" || _locationService.GetById(tourRequest.Location.Id).City == city;
            var isGuestNumberValid = string.IsNullOrEmpty(guestNumber) || tourRequest.GuestNumber >= Convert.ToInt32(guestNumber);
            var isLanguageValid = string.IsNullOrEmpty(language) || tourRequest.Language.ToLower().Contains(language.ToLower());

            return isLanguageValid && isGuestNumberValid && isCountryValid && isCityValid && isDateValid;
        }

        public void CreateRequest(string description, User guide, string language, Location location, int guestNumber, DateTime? fromDate, DateTime? toDate, TourRequestStatus requestStatus)
        {

            var request = new TourRequest(description, guide, language, location, guestNumber, fromDate, toDate, requestStatus, new ComplexTourRequest());
            _tourRequestRepository.Save(request);
        }

        public void UpdateStatus(TourRequest request)
        {
            request.RequestStatus = TourRequestStatus.Invalid;
            _tourRequestRepository.Update(request);
        }

        public bool IsPeriodExpired(TourRequest request)
        {
            var deadlineDate = request.FromDate?.Subtract(TimeSpan.FromDays(2));
            return deadlineDate < DateTime.Today && request.RequestStatus == TourRequestStatus.Pending;
        }

        public void CheckDateRange(TourRequest request)
        {
            if (IsPeriodExpired(request))
            {
                UpdateStatus(request);
            }
        }

        public List<TourRequest> GetAcceptedRequests()
        {
            List<TourRequest> accepted = new();
            foreach (var request in _tourRequestRepository.GetAll())
            {
                if (request.RequestStatus == TourRequestStatus.Accepted)
                {
                    accepted.Add(request);
                }
            }
            return accepted;
        }

        public List<TourRequest> GetRequestsByYear(string year)
        {
            List<TourRequest> requests = new();
            foreach (var request in _tourRequestRepository.GetAll().Where(request => request.FromDate != null && !requests.Contains(request)))
            {
                if (request.FromDate != null && year.Equals(request.FromDate.Value.Year.ToString()))
                {
                    requests.Add(request);
                }
            }
            return requests;
        }

        public List<Location> FindDistinctLocations()
        {
            var locations = new List<Location>();

            foreach (var request in _tourRequestRepository.GetAll())
            {
                foreach (var location in _locationService.GetAll())
                {
                    if (location.Id == request.Location.Id && !ExistsLocation(locations, location))
                    {
                        locations.Add(location);
                    }
                }
            }

            return locations;
        }

        public bool EverAcceptedLocation(Location location)
        {
            var accepted = GetAcceptedRequests();
            foreach (var request in accepted)
            {
                if (request.Location.Id == location.Id)
                {
                    return true;
                }
            }
            return false;
        }
        public bool EverAcceptedLanguage(string language)
        {
            var accepted = GetAcceptedRequests();
            foreach (var request in accepted)
            {
                if (request.Language.Equals(language))
                {
                    return true;
                }
            }
            return false;
        }
        public bool ExistsLocation(List<Location> locations, Location loc)
        {
            foreach (var location in locations)
            {
                if (loc.Id == location.Id)
                {
                    return true;
                }
            }
            return false;
        }

        public List<TourRequest> GetRequestsByComplexId(ComplexTourRequest complex){
            List<TourRequest> requestsById = new List<TourRequest>();
            foreach (TourRequest request in _tourRequestRepository.GetAll()) {
                if (request.ComplexTourRequest.Id == complex.Id) { 
                    requestsById.Add(request);
                }
            }
           return  requestsById;
        }

        public bool IsAllAccepted(List<TourRequest> requests) {
            foreach (TourRequest request in requests) {
                if ((int)(request.RequestStatus)!=2) {
                    return false;
                }
            }
            return true;
        }
    }
}
