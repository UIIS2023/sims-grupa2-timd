using System;
using SimsProject.Application.Interface;
using SimsProject.Application.UseCase;
using SimsProject.Domain.Model;
using SimsProject.WPF.View.Guide;

namespace SimsProject.WPF.ViewModel.Domain
{
    public class TourVoucherViewModel : ViewModelBase
    {
        private readonly ITourVoucherService _tourVoucherService;
        private readonly TourVoucher _tourVoucher;
        public TourViewModel Tour { get; }
        public UserViewModel Guest { get; }
        public DateTime ValidUntil => _tourVoucher.ValidUntil;
        public DateTime UsedOn => _tourVoucher.UsedOn;

        public TourVoucherViewModel(TourVoucher tourVoucher)
        {
            _tourVoucherService = Injector.CreateInstance<ITourVoucherService>();
            _tourVoucher = _tourVoucherService.GetById(tourVoucher.Id);
            Tour = new TourViewModel(_tourVoucher.Tour);
            Guest = new UserViewModel(_tourVoucher.Guest);
        }
    }
}
