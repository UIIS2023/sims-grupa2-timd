using SimsProject.Application.Interface;
using SimsProject.Domain.Model;

namespace SimsProject.WPF.ViewModel.Domain
{
    public class CheckPointViewModel : ViewModelBase
    {
        private readonly ICheckPointService _checkPointService;
        private readonly CheckPoint _checkPoint;

        public int Id
        {
            get => _checkPoint.Id;
            set => _checkPoint.Id = value;
        }

        public bool IsActive
        {
            get => _checkPoint.IsActive;
            set => _checkPoint.IsActive = value;
        }

        public int SerialNumber => _checkPoint.SerialNumber;
        public Tour Tour => _checkPoint.Tour;
        public string Name
        {
            get => _checkPoint.Name;
            set => _checkPoint.Name = value;
        }

        public CheckPointViewModel(CheckPoint checkPoint)
        {
            _checkPointService = Injector.CreateInstance<ICheckPointService>();
            _checkPoint = _checkPointService.GetById(checkPoint.Id);
        }
    }
}
