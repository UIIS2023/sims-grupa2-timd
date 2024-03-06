using SimsProject.Application.Interface;
using SimsProject.Domain.Model;

namespace SimsProject.WPF.ViewModel.Domain
{
    public class TourAttendanceViewModel
    {
        private readonly ITourAttendanceService _tourAttendanceService;
        private readonly TourAttendance _tourAttendance;

        public Presence Present
        {
            get => _tourAttendance.Present;
            set => _tourAttendance.Present = value;
        }

        public CheckPointViewModel CheckPoint { get; set; }
        public TourViewModel Tour { get; set; }
        public UserViewModel User { get; set; }

        public TourAttendanceViewModel(TourAttendance tourAttendance)
        {
            _tourAttendanceService = Injector.CreateInstance<ITourAttendanceService>();
            _tourAttendance = _tourAttendanceService.GetById(tourAttendance.Id);

            Tour = new TourViewModel(_tourAttendance.Tour);
            User = new UserViewModel(_tourAttendance.User);
            CheckPoint = new CheckPointViewModel(_tourAttendance.CheckPoint);
        }
    }
}
