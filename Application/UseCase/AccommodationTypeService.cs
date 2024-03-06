using SimsProject.Domain.RepositoryInterface;
using SimsProject.Domain.Model;
using SimsProject.Application.Interface;
using System.Collections.Generic;

namespace SimsProject.Application.UseCase
{
    public class AccommodationTypeService : IAccommodationTypeService
    {
        private readonly IAccommodationTypeRepository _typeRepository;

        public AccommodationTypeService()
        {
            _typeRepository = Injector.CreateInstance<IAccommodationTypeRepository>();
        }

        public AccommodationType GetById(int id)
        {
            return _typeRepository.GetById(id);
        }

        public List<AccommodationType> GetAll()
        {
            return _typeRepository.GetAll();
        }
    }
}
