using SimsProject.Domain.Model;
using System.Collections.Generic;

namespace SimsProject.Application.Interface
{
    public interface ITourVoucherService : IService<TourVoucher>
    {
        List<TourVoucher> GetAll();
        TourVoucher Save(TourVoucher tourVoucher);
        void Delete(TourVoucher tourVoucher);
        int CountGuestsWithVouchers(int tourId);
    }
}
