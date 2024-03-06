using System;
using System.Globalization;
using System.Windows.Data;

namespace SimsProject.WPF.View.Converter
{
    public class DateOnlyToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateOnly date)
            {
                return date.ToString("dd.MM.yyyy");
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
