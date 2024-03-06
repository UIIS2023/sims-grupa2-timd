using SimsProject.Application.Interface;
using SimsProject.Application.UseCase;
using SimsProject.Domain.RepositoryInterface;
using SimsProject.Repository;
using System;
using System.Collections.Generic;

namespace SimsProject
{
    public static class Injector
    {
        private static readonly Dictionary<Type, object> Implementations = new();

        public static void Initialize()
        {
            Implementations.Add(typeof(IUserRepository), new UserCsvRepository());
            Implementations.Add(typeof(ILocationRepository), new LocationCsvRepository());
            Implementations.Add(typeof(IAccommodationRepository), new AccommodationCsvRepository());
            Implementations.Add(typeof(IAccommodationTypeRepository), new AccommodationTypeCsvRepository());
            Implementations.Add(typeof(IAccommodationImageRepository), new AccommodationImageCsvRepository());
            Implementations.Add(typeof(IAccommodationReviewImageRepository), new AccommodationReviewImageCsvRepository());
            Implementations.Add(typeof(IAccommodationReservationRescheduleRepository), new AccommodationReservationRescheduleCsvRepository());
            Implementations.Add(typeof(IAccommodationReservationRepository), new AccommodationReservationCsvRepository());
            Implementations.Add(typeof(IAccommodationReviewRepository), new AccommodationReviewCsvRepository());
            Implementations.Add(typeof(IAccommodationRenovationRepository), new AccommodationRenovationCsvRepository());
            Implementations.Add(typeof(IGuestReviewRepository), new GuestReviewCsvRepository());
            Implementations.Add(typeof(IForumRepository), new ForumCsvRepository());
            Implementations.Add(typeof(IForumCommentRepository), new ForumCommentCsvRepository());
            Implementations.Add(typeof(IForumCommentReportRepository), new ForumCommentReportCsvRepository());
            Implementations.Add(typeof(ITourImageRepository), new TourImageCsvRepository());
            Implementations.Add(typeof(ITourReviewImageRepository), new TourReviewImageCsvRepository());
            Implementations.Add(typeof(ITourRepository), new TourCsvRepository());
            Implementations.Add(typeof(ITourVoucherRepository), new TourVoucherCsvRepository());
            Implementations.Add(typeof(ITourDateRepository), new TourDateCsvRepository());
            Implementations.Add(typeof(ICheckPointRepository), new CheckPointCsvRepository());
            Implementations.Add(typeof(ITourReservationRepository), new TourReservationCsvRepository());
            Implementations.Add(typeof(ITourReviewRepository), new TourReviewCsvRepository());
            Implementations.Add(typeof(ITourAttendanceRepository), new TourAttendanceCsvRepository());
            Implementations.Add(typeof(INotificationRepository), new NotificationCsvRepository());
            Implementations.Add(typeof(ITourRequestRepository), new TourRequestCsvRepository());
            Implementations.Add(typeof(IComplexTourRequestRepository), new ComplexTourRequestCsvRepository());

            Implementations.Add(typeof(ILocationService), new LocationService());
            Implementations.Add(typeof(IAccommodationImageService), new AccommodationImageService());
            Implementations.Add(typeof(IAccommodationReviewImageService), new AccommodationReviewImageService());
            Implementations.Add(typeof(IAccommodationTypeService), new AccommodationTypeService());
            Implementations.Add(typeof(IAccommodationService), new AccommodationService());
            Implementations.Add(typeof(IAccommodationRenovationService), new AccommodationRenovationService());
            Implementations.Add(typeof(IAccommodationReservationService), new AccommodationReservationService());
            Implementations.Add(typeof(IAccommodationReservationRescheduleService), new AccommodationReservationRescheduleService());
            Implementations.Add(typeof(IAccommodationReviewService), new AccommodationReviewService());
            Implementations.Add(typeof(IGuestReviewService), new GuestReviewService());
            Implementations.Add(typeof(IForumCommentReportService), new ForumCommentReportService());
            Implementations.Add(typeof(IForumCommentService), new ForumCommentService());
            Implementations.Add(typeof(IReviewService), new ReviewService());
            Implementations.Add(typeof(IAccommodationStatisticsService), new AccommodationStatisticsService());
            Implementations.Add(typeof(IOwnerService), new OwnerService());
            Implementations.Add(typeof(ITourDateService), new TourDateService());
            Implementations.Add(typeof(ITourReviewImageService), new TourReviewImageService());
            Implementations.Add(typeof(ITourVoucherService), new TourVoucherService());
            Implementations.Add(typeof(ICheckPointService), new CheckPointService());
            Implementations.Add(typeof(ITourAttendanceService), new TourAttendanceService());
            Implementations.Add(typeof(ITourImageService), new TourImageService());
            Implementations.Add(typeof(ITourService), new TourService());
            Implementations.Add(typeof(ITourReviewService), new TourReviewService());
            Implementations.Add(typeof(ITourReservationService), new TourReservationService());
            Implementations.Add(typeof(IGuideService), new GuideService());
            Implementations.Add(typeof(IUserService), new UserService());
            Implementations.Add(typeof(IGuest1Service), new Guest1Service());
            Implementations.Add(typeof(IForumService), new ForumService());
            Implementations.Add(typeof(IOwnerNotificationService), new OwnerNotificationService());
            Implementations.Add(typeof(ITourRequestService), new TourRequestService());
            Implementations.Add(typeof(IGuest2NotificationService), new Guest2NotificationService());
            Implementations.Add(typeof(IComplexTourRequestService), new ComplexTourRequestService());
        }

        public static T CreateInstance<T>()
        {
            var type = typeof(T);
        
            if (Implementations.ContainsKey(type))
            {
                return (T)Implementations[type];
            }
        
            throw new ArgumentException($"No implementation found for type {type}");
        }
    }
}
