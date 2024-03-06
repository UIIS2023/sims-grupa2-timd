using System.Collections.Generic;
using System.Linq;
using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.Domain.RepositoryInterface;

namespace SimsProject.Application.UseCase
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IOwnerService _ownerService;
        private readonly IGuideService _guideService;
        private readonly IAccommodationService _accommodationService;
        private readonly IAccommodationReservationService _accommodationReservationService;
        private readonly ITourReservationService _tourReservationService;
        private readonly ITourService _tourService;

        public UserService()
        {
            _userRepository = Injector.CreateInstance<IUserRepository>();
            _ownerService = Injector.CreateInstance<IOwnerService>();
            _guideService = Injector.CreateInstance<IGuideService>();
            _accommodationService = Injector.CreateInstance<IAccommodationService>();
            _accommodationReservationService = Injector.CreateInstance<IAccommodationReservationService>();
            _tourReservationService = Injector.CreateInstance<ITourReservationService>();
            _tourService = Injector.CreateInstance<ITourService>();
        }

        public User GetById(int id)
        {
            return _userRepository.GetById(id);
        }

        public List<User> GetAll() {
            return _userRepository.GetAll();
        }

        public User GetByUsername(string username)
        {
            var user = _userRepository.GetByUsername(username);
            return user;
        }

        public User CreateUser(User user)
        {
            return _userRepository.GetByUsername(user.Username) is not null ? null : _userRepository.Save(user);
        }

        public User Update(User user)
        {
            return _userRepository.Update(user);
        }

        public bool ChangePassword(User user, string currentPassword, string newPassword)
        {
            if (user.Password != currentPassword) return false;

            user.Password = newPassword;
            _userRepository.Update(user);
            return true;
        }

        public void UpdateAllSuperUserStatuses()
        {
            foreach (var user in _userRepository.GetAll()
                                                .Where(user => user.Type is UserType.Owner or UserType.Guide)
                                                .Where(user => user.IsSuperUser != GetUpdatedSuperUserStatus(user)))
            {
                UpdateSuperUserStatus(user);
            }
        }

        public void Delete(User entity)
        {
            _userRepository.Delete(entity);
        }

        private bool GetUpdatedSuperUserStatus(User user)
        {
            if (user.Type == UserType.Owner)
            {
                return _ownerService.GetSuperOwnerStatus(user);
            }

            if(user.Type == UserType.Guide)
            {
                return _guideService.GetSuperGuideStatus(user);
            }

            return false;
        }

        private void UpdateSuperUserStatus(User user)
        {
            user.IsSuperUser = !user.IsSuperUser;
            _userRepository.Update(user);
        }

        public bool IsAuthorValid(User user, int locationId)
        {
            if (user.Type == UserType.Owner)
            {
                var accommodations = _accommodationService.GetByOwner(user);
                return accommodations.Any(accommodation => accommodation.Location.Id == locationId);
            }
            if (user.Type == UserType.Guest1)
            {
                var reservations = _accommodationReservationService.GetByGuest(user);
                return reservations.Any(reservation => _accommodationService.GetById(reservation.Accommodation.Id).Location.Id == locationId);
            }
            if (user.Type == UserType.Guest2)
            {
                var tours = _tourReservationService.GetByGuest(user);
                return tours.Any(tour => _tourService.GetById(tour.Tour.Id).TourLocation.Id == locationId);
            }
            return false;
        }

    }
}
