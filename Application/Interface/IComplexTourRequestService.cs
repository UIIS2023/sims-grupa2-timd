using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimsProject.Domain.Model;

namespace SimsProject.Application.Interface
{
    public interface IComplexTourRequestService : IService<ComplexTourRequest>
    {
        List<ComplexTourRequest> GetAll();
        public void CreateComplexRequest(List<TourRequest> requests, ComplexRequestStatus status);
        public void SavePartialRequestst(List<TourRequest> requests, ComplexTourRequest complexRequest);
        public void Update(ComplexTourRequest complex);


    }
}
