using System;
using System.Collections.Generic;
using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.Domain.RepositoryInterface;
using System.Linq;

namespace SimsProject.Application.UseCase
{
    public class OwnerNotificationService : IOwnerNotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IAccommodationReservationService _reservationService;
        private readonly IReviewService _reviewService;
        private readonly IAccommodationReservationRescheduleService _reservationRescheduleService;
        private readonly IAccommodationService _accommodationService;
        private readonly IUserService _userService;
        private readonly ILocationService _locationService;
        private readonly IForumService _forumService;

        public OwnerNotificationService()
        {
            _notificationRepository = Injector.CreateInstance<INotificationRepository>();
            _reservationService = Injector.CreateInstance<IAccommodationReservationService>();
            _reviewService = Injector.CreateInstance<IReviewService>();
            _reservationRescheduleService = Injector.CreateInstance<IAccommodationReservationRescheduleService>();
            _accommodationService = Injector.CreateInstance<IAccommodationService>();
            _userService = Injector.CreateInstance<IUserService>();
            _locationService = Injector.CreateInstance<ILocationService>();
            _forumService = Injector.CreateInstance<IForumService>();
        }

        public Notification GetById(int id)
        {
            return _notificationRepository.GetById(id);
        }

        public List<Notification> GetByUser(User user)
        {
            return _notificationRepository.GetByUser(user);
        }

        public List<Notification> GetInboxByUser(User owner)
        {
            var notifications = new List<Notification>();
            var unreadNotifications = _notificationRepository.GetUnreadByUser(owner);
            var newNotifications = GenerateNewNotifications(owner);

            notifications.AddRange(newNotifications);
            notifications.AddRange(unreadNotifications);

            return notifications;
        }

        private List<Notification> GenerateNewNotifications(User owner)
        {
            var newNotifications = new List<Notification>();
            newNotifications.AddRange(GenerateOpenedForumNotifications(owner));
            newNotifications.AddRange(GenerateReviewNotifications(owner));
            newNotifications.AddRange(GenerateCanceledNotifications(owner));
            newNotifications.AddRange(GenerateRescheduleNotifications(owner));
            return newNotifications;
        }

        private List<Notification> GenerateReviewNotifications(User owner)
        {
            var existingNotificationMessages = (from notification in _notificationRepository.GetByUser(owner)
                                                select notification.Message).ToList();

            var notifications = new List<Notification>();
            foreach (var reservation in _reviewService.GetUnreviewedByOwner(owner))
            {
                var notification = WriteAccommodationNotification(owner, reservation, NotificationType.Review);
                if (!existingNotificationMessages.Contains(notification.Message))
                {
                    notifications.Add(_notificationRepository.Save(notification));
                }
            }
            return notifications;
        }

        private List<Notification> GenerateCanceledNotifications(User owner)
        {
            var notifications = new List<Notification>();
            foreach (var reservation in _reservationService.GetCanceledByOwner(owner)
                                                           .Where(reservation => reservation.NotifyOwnerOfCancel))
            {
                UpdateReservationNotificationStatus(reservation);
                var notification = WriteAccommodationNotification(owner, reservation, NotificationType.Cancel);
                notifications.Add(_notificationRepository.Save(notification));
            }
            return notifications;
        }

        private List<Notification> GenerateRescheduleNotifications(User owner)
        {
            var notifications = new List<Notification>();
            foreach (var request in _reservationRescheduleService.GetByOwner(owner)
                                                                 .Where(request => request.NotifyUserOfStatusChange && request.RequestStatus == Status.Waiting))
            {
                UpdateRescheduleNotificationStatus(request);
                var notification = WriteAccommodationNotification(owner, request.Reservation, NotificationType.Reschedule);
                notifications.Add(_notificationRepository.Save(notification));
            }
            return notifications;
        }

        private List<Notification> GenerateOpenedForumNotifications(User owner)
        {
            var existingNotificationMessages = (from notification in _notificationRepository.GetByUser(owner)
                select notification.Message).ToList();

            var notifications = new List<Notification>();
            foreach (var forum in _forumService.GetAll().Where(f => _userService.IsAuthorValid(owner, f.Location.Id)))
            {
                var notification = WriteForumNotification(owner, forum);
                if (!existingNotificationMessages.Contains(notification.Message))
                {
                    notifications.Add(_notificationRepository.Save(notification));
                }
            }
            return notifications;
        }

        private void UpdateReservationNotificationStatus(AccommodationReservation reservation)
        {
            reservation.NotifyOwnerOfCancel = false;
            _reservationService.Update(reservation);
        }

        private void UpdateRescheduleNotificationStatus(AccommodationReservationReschedule request)
        {
            request.NotifyUserOfStatusChange = false;
            _reservationRescheduleService.Update(request);
        }

        private Notification WriteAccommodationNotification(User owner, AccommodationReservation reservation, NotificationType type)
        {
            var deadlineDate = reservation.CheckoutDate.AddDays(5).ToDateTime(TimeOnly.MinValue);
            var daysLeftToReview = (int)(deadlineDate - DateTime.Today).TotalDays;

            reservation = _reservationService.GetById(reservation.Id);
            var guest = _userService.GetById(reservation.Guest.Id);
            var accommodation = _accommodationService.GetById(reservation.Accommodation.Id);

            var message = WriteMessage(accommodation, reservation, guest, daysLeftToReview, type);

            return new Notification(owner, message, type);
        }

        private static string WriteMessage(Accommodation accommodation, AccommodationReservation reservation, User guest, int daysLeftToReview, NotificationType type)
        {
            return type switch
            {
                NotificationType.Review => $"Guest {guest.Username} has checked out on {reservation.CheckoutDate.ToString("dd.MM.yyyy")} from {accommodation.Name}. Days left to rate their stay: {daysLeftToReview}",
                NotificationType.Cancel => $"Guest {guest.Username} has canceled their reservation that starts on {reservation.ArrivalDate.ToString("dd.MM.yyyy")} at {accommodation.Name}.",
                NotificationType.Reschedule => $"Guest {guest.Username} has requested to reschedule their reservation that starts on {reservation.ArrivalDate.ToString("dd.MM.yyyy")} at {accommodation.Name}.",
                _ => throw new ArgumentException("Invalid notification type.", nameof(type)),
            };
        }
        private Notification WriteForumNotification(User owner, Forum forum)
        {
            var author = _userService.GetById(forum.Creator.Id);
            var location = _locationService.GetById(forum.Location.Id);

            var message = $"{author.Username} opened a forum about {location}. Check it out, and leave a comment!";

            return new Notification(owner, message, NotificationType.Forum);
        }

        public void ReadNotification(int notificationId)
        {
            var notification = _notificationRepository.GetById(notificationId);
            notification.IsRead = true;
            _notificationRepository.Update(notification);
        }
    }
}