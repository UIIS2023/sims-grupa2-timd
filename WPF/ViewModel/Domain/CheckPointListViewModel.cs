using System.Collections.ObjectModel;
using SimsProject.Application.Interface;
using SimsProject.Domain.Model;

namespace SimsProject.WPF.ViewModel.Domain
{
    public class CheckPointListViewModel : ViewModelBase
    {
        public ObservableCollection<CheckPointViewModel> CheckPoints { get; set; }
        private readonly ICheckPointService _checkPointService;

        public CheckPointListViewModel(Tour tour)
        {
            _checkPointService = Injector.CreateInstance<ICheckPointService>();
            CheckPoints = new ObservableCollection<CheckPointViewModel>();

            foreach (var checkPoint in _checkPointService.GetByParentId(tour.Id))
            {
                CheckPoints.Add(new CheckPointViewModel(checkPoint));
            }

        }
    }
}
