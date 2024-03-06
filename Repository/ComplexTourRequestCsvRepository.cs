using SimsProject.Domain.Model;
using SimsProject.Domain.RepositoryInterface;
using SimsProject.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimsProject.Repository
{
    public class ComplexTourRequestCsvRepository : IComplexTourRequestRepository
    {
        private const string FilePath = "../../../Resources/Data/complexTourRequests.csv";

        private readonly Serializer<ComplexTourRequest> _serializer;
        private List<ComplexTourRequest> _complexTourRequests;

        public ComplexTourRequestCsvRepository()
        {
            _serializer = new Serializer<ComplexTourRequest>();
            _complexTourRequests = _serializer.FromCsv(FilePath);
        }

        public List<ComplexTourRequest> GetAll()
        {
            return _serializer.FromCsv(FilePath);
        }

        public ComplexTourRequest GetById(int id)
        {
            _complexTourRequests = _serializer.FromCsv(FilePath);
            return _complexTourRequests.FirstOrDefault(a => a.Id == id);
        }

        public int NextId()
        {
            _complexTourRequests = _serializer.FromCsv(FilePath);
            if (_complexTourRequests.Count < 1)
            {
                return 1;
            }
            return _complexTourRequests.Max(t => t.Id) + 1;
        }

        public ComplexTourRequest Save(ComplexTourRequest request)
        {
            request.Id = NextId();
            _complexTourRequests = _serializer.FromCsv(FilePath);
            _complexTourRequests.Add(request);
            _serializer.ToCsv(FilePath, _complexTourRequests);
            return request;
        }

        public ComplexTourRequest Update(ComplexTourRequest request)
        {
            _complexTourRequests = _serializer.FromCsv(FilePath);
            var current = _complexTourRequests.Find(t => t.Id == request.Id);
            var index = _complexTourRequests.IndexOf(current);
            _complexTourRequests.Remove(current);
            _complexTourRequests.Insert(index, request);
            _serializer.ToCsv(FilePath, _complexTourRequests);

            return request;
        }

        public void Delete(ComplexTourRequest request)
        {
            _complexTourRequests = _serializer.FromCsv(FilePath);
            var found = _complexTourRequests.Find(a => a.Id == request.Id);
            _complexTourRequests.Remove(found);
            _serializer.ToCsv(FilePath, _complexTourRequests);
        }
    }
}
