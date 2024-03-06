using SimsProject.Application.Interface;
using SimsProject.Domain.Model;

namespace SimsProject.WPF.ViewModel.Domain
{
    public class LocationViewModel : ViewModelBase
    {
        private readonly Location _location;

        public int Id => _location.Id;
        public string City => _location.City;
        public string Country => _location.Country;

        public LocationViewModel(Location location)
        {
            var locationService = Injector.CreateInstance<ILocationService>();
            _location = locationService.GetById(location.Id);
        }

        public override string ToString()
        {
            return $"{City}, {Country}";
        }
    }
}
