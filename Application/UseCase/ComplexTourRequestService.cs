using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.Domain.RepositoryInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimsProject.Application.UseCase
{
    public class ComplexTourRequestService : IComplexTourRequestService
    {
        private readonly IComplexTourRequestRepository _complexTourRequestRepository;
        public readonly ITourRequestRepository _tourRequestRepository;


        public ComplexTourRequestService()
        {
            _complexTourRequestRepository = Injector.CreateInstance<IComplexTourRequestRepository>();
            _tourRequestRepository = Injector.CreateInstance<ITourRequestRepository>();

        }

        public ComplexTourRequest GetById(int id)
        {
            return _complexTourRequestRepository.GetById(id);
        }

        public List<ComplexTourRequest> GetAll()
        {
            return _complexTourRequestRepository.GetAll();
        }

        public void CreateComplexRequest(List<TourRequest> requests,ComplexRequestStatus status) {
            var complexRequest = new ComplexTourRequest(status);
            complexRequest.TourRequests = requests;
            _complexTourRequestRepository.Save(complexRequest);
            SavePartialRequestst(requests, complexRequest);
        }

        public void SavePartialRequestst(List<TourRequest> requests,ComplexTourRequest complexRequest) {
            foreach (TourRequest request in requests) {
                request.ComplexTourRequest.Id = complexRequest.Id;
                _tourRequestRepository.Save(request);
            }
        }

        public void Update(ComplexTourRequest complex) {
            _complexTourRequestRepository.Update(complex);
        }
    }
}
