using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Data;

namespace SimsProject.WPF.View.Converter
{
    public class EnumDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var displayValue = string.Empty;

            try
            {
                var type = value.GetType();

                if (!type.IsEnum)
                    throw new ArgumentException("Type provided must be an Enum.");

                var enumValue = value.ToString();

                if (enumValue != null)
                {
                    var memberInfo = type.GetMember(enumValue).FirstOrDefault();

                    if (memberInfo != null) 
                    {
                        var displayAttr = memberInfo.GetCustomAttribute<DisplayAttribute>();

                        if (displayAttr != null)
                            displayValue = displayAttr.Name;
                    }
                }

                if (!string.IsNullOrEmpty(displayValue)) return displayValue;
                if (enumValue != null)
                    displayValue = string.Concat(enumValue.Select((x, i) =>
                        i > 0 && char.IsUpper(x) ? " " + x.ToString() : x.ToString()));
                return displayValue;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
