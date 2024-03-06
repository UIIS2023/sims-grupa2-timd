using System.Collections.Generic;
using System.Linq;
using SimsProject.Domain.Model;
using SimsProject.Domain.RepositoryInterface;
using SimsProject.Serializer;

namespace SimsProject.Repository
{
    public class TourVoucherCsvRepository : ITourVoucherRepository
    {
        private const string FilePath = "../../../Resources/Data/tourVouchers.csv";

        private readonly Serializer<TourVoucher> _serializer;
        private List<TourVoucher> _tourVouchers;

        public TourVoucherCsvRepository()
        {
            _serializer = new Serializer<TourVoucher>();
            _tourVouchers = _serializer.FromCsv(FilePath);
        }

        public List<TourVoucher> GetAll()
        {
            var tourVouchers = _serializer.FromCsv(FilePath);
            return tourVouchers;
        }

        public TourVoucher GetById(int id)
        {
            _tourVouchers = _serializer.FromCsv(FilePath);
            return _tourVouchers.FirstOrDefault(l => l.Id == id);
        }

        public int NextId()
        {
            _tourVouchers = GetAll();
            if (_tourVouchers.Count < 1)
            {
                return 1;
            }
            return _tourVouchers.Max(v => v.Id) + 1;
        }

        public TourVoucher Save(TourVoucher tourVoucher)
        {
            tourVoucher.Id = NextId();
            _tourVouchers = GetAll();
            _tourVouchers.Add(tourVoucher);
            _serializer.ToCsv(FilePath, _tourVouchers);
            return tourVoucher;
        }

        public TourVoucher Update(TourVoucher tourVoucher)
        {
            _tourVouchers = _serializer.FromCsv(FilePath);
            var current = _tourVouchers.Find(u => u.Id == tourVoucher.Id);
            var index = _tourVouchers.IndexOf(current);
            _tourVouchers.Remove(current);
            _tourVouchers.Insert(index, tourVoucher);
            _serializer.ToCsv(FilePath, _tourVouchers);
            return tourVoucher;
        }

        public void Delete(TourVoucher tourVoucher)
        {
            _tourVouchers = _serializer.FromCsv(FilePath);
            var found = _tourVouchers.Find(a => a.Id == tourVoucher.Id);
            _tourVouchers.Remove(found);
            _serializer.ToCsv(FilePath, _tourVouchers);
        }
    }
}
