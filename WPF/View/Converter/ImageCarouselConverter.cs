using SimsProject.WPF.ViewModel.Domain;
using SimsProject.WPF.ViewModel.Guide;
using System;
using System.Globalization;
using System.Windows.Data;

namespace SimsProject.WPF.ViewModel
{
    public class ImageCarouselConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TourViewModel tourViewModel)
            {
                if (MainPanelViewModel.ImageCarouselViewModels.TryGetValue(tourViewModel, out var imageCarouselViewModel))
                {
                    return imageCarouselViewModel;
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
