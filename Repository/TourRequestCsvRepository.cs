using System.Collections.Generic;
using System.Linq;
using SimsProject.Domain.Model;
using SimsProject.Domain.RepositoryInterface;
using SimsProject.Serializer;

namespace SimsProject.Repository
{
    public class TourRequestCsvRepository : ITourRequestRepository
    {
        private const string FilePath = "../../../Resources/Data/tourRequests.csv";

        private readonly Serializer<TourRequest> _serializer;
        private List<TourRequest> _tourRequests;

        public TourRequestCsvRepository()
        {
            _serializer = new Serializer<TourRequest>();
            _tourRequests = _serializer.FromCsv(FilePath);
        }

        public List<TourRequest> GetAll()
        {
            return _serializer.FromCsv(FilePath);
        }

        public TourRequest GetById(int id)
        {
            _tourRequests = _serializer.FromCsv(FilePath);
            return _tourRequests.FirstOrDefault(a => a.Id == id);
        }

        public List<TourRequest> GetByParentId(int parentId)
        {
            _tourRequests = GetAll();
            return _tourRequests.FindAll(t => t.ComplexTourRequest.Id == parentId);
        }

        public int NextId()
        {
            _tourRequests = _serializer.FromCsv(FilePath);
            if (_tourRequests.Count < 1)
            {
                return 1;
            }
            return _tourRequests.Max(t => t.Id) + 1;
        }

        public TourRequest Save(TourRequest tour)
        {
            tour.Id = NextId();
            _tourRequests = _serializer.FromCsv(FilePath);
            _tourRequests.Add(tour);
            _serializer.ToCsv(FilePath, _tourRequests);
            return tour;
        }

        public TourRequest Update(TourRequest request)
        {
            _tourRequests = _serializer.FromCsv(FilePath);
            var current = _tourRequests.Find(t => t.Id == request.Id);
            var index = _tourRequests.IndexOf(current);
            _tourRequests.Remove(current);
            _tourRequests.Insert(index, request);
            _serializer.ToCsv(FilePath, _tourRequests);

            return request;
        }

        public void Delete(TourRequest request)
        {
            _tourRequests = _serializer.FromCsv(FilePath);
            var found = _tourRequests.Find(a => a.Id == request.Id);
            _tourRequests.Remove(found);
            _serializer.ToCsv(FilePath, _tourRequests);
        }

        public List<TourRequest> GetByGuide(User guide)
        {
            _tourRequests = _serializer.FromCsv(FilePath);
            return _tourRequests.FindAll(a => a.Guide.Id == guide.Id);
        }

        public TourRequest UpdateById(int tourRequestId)
        {
            _tourRequests = GetAll();
            var currentReview = _tourRequests.Find(d => d.Id == tourRequestId);
            currentReview.RequestStatus = TourRequestStatus.Accepted;
            var index = _tourRequests.IndexOf(currentReview);

            _tourRequests.Remove(currentReview);
            _tourRequests.Insert(index, currentReview);
            _serializer.ToCsv(FilePath, _tourRequests);
            return currentReview;
        }
    }
}
