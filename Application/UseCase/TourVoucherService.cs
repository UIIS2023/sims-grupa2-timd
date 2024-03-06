using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.Domain.RepositoryInterface;
using System.Collections.Generic;

namespace SimsProject.Application.UseCase
{
    public class TourVoucherService : ITourVoucherService
    {
        private readonly ITourVoucherRepository _tourVoucherRepository;
        private readonly ITourDateService _tourDateService;

        public TourVoucherService()
        {
            _tourVoucherRepository = Injector.CreateInstance<ITourVoucherRepository>();
            _tourDateService = Injector.CreateInstance<ITourDateService>();
        }

        public TourVoucher GetById(int id)
        {
            return _tourVoucherRepository.GetById(id);
        }

        public List<TourVoucher> GetAll()
        {
            return _tourVoucherRepository.GetAll();
        }

        public TourVoucher Save(TourVoucher tourVoucher)
        {
            return _tourVoucherRepository.Save(tourVoucher);
        }

        public List<TourVoucher> GetByGuest(User guest)
        {
            return _tourVoucherRepository.GetAll().FindAll(v => v.Guest.Id == guest.Id);
        }

        public void Delete(TourVoucher tourVoucher) 
        {
            _tourVoucherRepository.Delete(tourVoucher);
        }

        public int CountGuestsWithVouchers(int tourId)
        {
            var guestsWithVouchers = 0;
            foreach (var voucher in _tourVoucherRepository.GetAll())
            {
                foreach (var date in _tourDateService.GetAll())
                {
                    if (voucher.UsedOn == date.Date && voucher.Tour.Id == tourId && date.HasEnded &&
                        date.Tour.Id == tourId)
                    {
                        guestsWithVouchers++;
                    }
                }
            }
            return guestsWithVouchers;
        }
    }
}
