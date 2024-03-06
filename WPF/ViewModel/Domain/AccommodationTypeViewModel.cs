using SimsProject.Application.Interface;
using SimsProject.Domain.Model;

namespace SimsProject.WPF.ViewModel.Domain
{
    public class AccommodationTypeViewModel : ViewModelBase
    {
        private readonly AccommodationType _type;

        internal int Id => _type.Id;
        public string Name => _type.Name;
        public bool IsSelected { get; set; }

        public AccommodationTypeViewModel(AccommodationType type)
        {
            var accommodationTypeService = Injector.CreateInstance<IAccommodationTypeService>();
            _type = accommodationTypeService.GetById(type.Id);
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
