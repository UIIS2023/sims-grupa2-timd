using System;
using System.Globalization;
using System.Windows.Data;

namespace SimsProject.WPF.View.Converter
{
    public class IntToMonthNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not int monthNumber) return null;
            return monthNumber is < 1 or > 12
                ? null
                : (object)CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthNumber);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
