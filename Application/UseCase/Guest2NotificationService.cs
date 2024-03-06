using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.Domain.RepositoryInterface;
using System.Collections.Generic;

namespace SimsProject.Application.UseCase
{
    public class Guest2NotificationService : IGuest2NotificationService
    {

        private readonly INotificationRepository _notificationRepository;
        private readonly IUserService _userService;
        private readonly ITourRequestService _tourRequestService;



        public Guest2NotificationService()
        {
            _notificationRepository = Injector.CreateInstance<INotificationRepository>();
            _userService = Injector.CreateInstance<IUserService>();
            _tourRequestService = Injector.CreateInstance<ITourRequestService>();
        }

        public void ReadNotification(int notificationId)
        {
            var notification = _notificationRepository.GetById(notificationId);
            notification.IsRead = true;
            _notificationRepository.Update(notification);
        }

        public Notification GetById(int id)
        {
            return _notificationRepository.GetById(id);
        }

        public List<Notification> GetByUser(User user)
        {
            return _notificationRepository.GetByUser(user);
        }

        public void NotifyGuest2(Tour newTour, int requestId) {
            User guest2 = _userService.GetById(4);
            var notification = new Notification();
            notification.Type = NotificationType.Request;
            notification.IsRead = false;
            notification.User = guest2;
            notification.Message = $"Your request with ID {requestId} has been accepted and new tour with TourID {newTour.Id} has been created!";
            if (ShouldNotify(newTour)) {
                _notificationRepository.Save(notification);
            }

            TourRequest request = _tourRequestService.GetById(requestId);
            request.RequestStatus = TourRequestStatus.Accepted;
             _tourRequestService.UpdateById(requestId);
        }

        public bool ShouldNotify(Tour newTour) {
            return (!_tourRequestService.EverAcceptedLocation(newTour.TourLocation)) || (!_tourRequestService.EverAcceptedLanguage(newTour.Language));
             
        }

       
    }
}


