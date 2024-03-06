using System;
using System.Windows.Navigation;
using SimsProject.Application.Interface;
using SimsProject.Application.Localization;
using SimsProject.Domain.Model;

namespace SimsProject.WPF.ViewModel.Guest1
{
    public class ProfilePageViewModel
    {
        public User CurrentUser { get; set; }
        public NavigationService NavigationService { get; set; }
        public string CurrentUserType { get; set; }
        public int Points { get; set; }
        public DateOnly Date { get; set; }
        public int Number { get; set; }
        public int Stat1 { get; set; }
        public double Stat2 { get; set; }
        public double Stat3 { get; set; }
        public bool IsNotSuperUser { get; set; }
        private IGuest1Service _guest1Service;
        
        public ProfilePageViewModel(User currentUser, NavigationService navigationService)
        {
            CurrentUser = currentUser;
            NavigationService = navigationService;
            _guest1Service = Injector.CreateInstance<IGuest1Service>();
            CurrentUserType = CurrentUser.IsSuperUser ? TranslationSource.Instance["SuperGuest"] : TranslationSource.Instance["OrdinaryGuest"];
            IsNotSuperUser = !CurrentUser.IsSuperUser;
            if (CurrentUser.SuperUserPoints != null) Points = (int)CurrentUser.SuperUserPoints;
            Date = CurrentUser.BecameSuperUser.AddYears(1);
            Number = CurrentUser.IsSuperUser ? _guest1Service.GetNumberOfReservationsPastPeriod(CurrentUser) : _guest1Service.GetNumberOfReservationsPastYear(CurrentUser);
            Stat1 = _guest1Service.GetNumberOfReservations(CurrentUser);
            Stat2 = _guest1Service.GetAverageCleanliness(currentUser);
            Stat3 = _guest1Service.GetAverageObservance(currentUser);
        }
    }
}
