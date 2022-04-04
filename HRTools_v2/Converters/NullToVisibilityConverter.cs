using HRTools_v2.Converters.Base;
using System;
using System.Globalization;
using System.Windows;

namespace HRTools_v2.Converters
{
    public class NullToVisibilityConverter : BaseValueConverter<NullToVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
            {
                return value == null ? Visibility.Collapsed : Visibility.Visible;
            }
            else if (parameter.Equals("tooltip"))
            {
                return string.IsNullOrEmpty(value.ToString()) ? Visibility.Collapsed : Visibility.Visible;
            }

            return value != null ? Visibility.Collapsed : Visibility.Visible;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
