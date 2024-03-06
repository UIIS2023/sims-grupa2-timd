using SimsProject.Application.UseCase;
using SimsProject.Domain.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SimsProject.WPF.ViewModel.Guest2
{
    public  class ShowCheckPointViewModel : INotifyPropertyChanged
    {
        User LoggedInUser;
        public CheckPoint MyCheckPoint;
        public CheckPoint ActiveCheckPoint;
        public TourAttendanceService tourAttendanceService;

        public string _checkPointName;
        public string CheckPointName
        {
            get => _checkPointName;
            set
            {
                if (value != _checkPointName)
                {
                    _checkPointName = value;
                    OnPropertyChanged(nameof(CheckPointName));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public ShowCheckPointViewModel(User currentUser) { 
            LoggedInUser = currentUser;
            tourAttendanceService = new();
            MyCheckPoint = tourAttendanceService.GetMyCheckPoint(LoggedInUser);
            SetCheckPointName(MyCheckPoint);
           
            
        }

        public void SetCheckPointName(CheckPoint point) {
            if (point.IsActive)
            {
                ActiveCheckPoint = point;
                CheckPointName = ActiveCheckPoint.Name;
            }
            else {
                CheckPointName = String.Empty;
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
