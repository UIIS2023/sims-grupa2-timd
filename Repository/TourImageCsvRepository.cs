using SimsProject.Domain.Model;
using SimsProject.Serializer;
using System.Collections.Generic;
using System.Linq;
using SimsProject.Domain.RepositoryInterface;

namespace SimsProject.Repository
{
    public class TourImageCsvRepository : ITourImageRepository
    {
        private const string FilePath = "../../../Resources/Data/tourImages.csv";

        private readonly Serializer<Image> _serializer;

        private List<Image> _images;

        public TourImageCsvRepository()
        {
            _serializer = new Serializer<Image>();
            _images = _serializer.FromCsv(FilePath);
        }

        public List<Image> GetAll()
        {
            return _serializer.FromCsv(FilePath);
        }

        public Image GetById(int id)
        {
            _images = _serializer.FromCsv(FilePath);
            return _images.FirstOrDefault(l => l.Id == id);
        }

        public int NextId()
        {
            _images = _serializer.FromCsv(FilePath);
            if (_images.Count < 1)
            {
                return 1;
            }
            return _images.Max(i => i.Id) + 1;
        }

        public Image Save(Image image)
        {
            image.Id = NextId();
            _images = _serializer.FromCsv(FilePath);
            _images.Add(image);
            _serializer.ToCsv(FilePath, _images);
            return image;
        }

        public Image Update(Image image)
        {
            _images = _serializer.FromCsv(FilePath);
            Image current = _images.Find(u => u.Id == image.Id);
            int index = _images.IndexOf(current);
            _images.Remove(current);
            _images.Insert(index, image);
            _serializer.ToCsv(FilePath, _images);
            return image;
        }

        public void Delete(Image image)
        {
            _images = _serializer.FromCsv(FilePath);
            Image found = _images.Find(a => a.Id == image.Id);
            _images.Remove(found);
            _serializer.ToCsv(FilePath, _images);
        }

        public List<Image> GetByParentId(int parentId)
        {
            _images = _serializer.FromCsv(FilePath);
            return _images.FindAll(i => i.ParentId == parentId);
        }

        public List<Image> SaveAllByParentId(int parentId, List<Image> images)
        {
            var savedImages = new List<Image>();
            foreach (var image in images)
            {
                image.ParentId = parentId;
                savedImages.Add(Save(image));
            }

            return savedImages;
        }

        public void DeleteAllByParentId(int parentId)
        {
            _images = _serializer.FromCsv(FilePath);
            var found = _images.FindAll(i => i.ParentId == parentId);
            foreach (var image in found)
            {
                _images.Remove(image);
            }
            _serializer.ToCsv(FilePath, _images);
        }
    }
}