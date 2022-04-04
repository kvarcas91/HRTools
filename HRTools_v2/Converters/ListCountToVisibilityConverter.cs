using HRTools_v2.Converters.Base;
using System;
using System.Globalization;
using System.Windows;

namespace HRTools_v2.Converters
{
    public class ListCountToVisibilityConverter : BaseValueConverter<ListCountToVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                int val = System.Convert.ToInt32(value);
                return val > 0 ? Visibility.Visible : Visibility.Hidden;
            }
            catch
            {
                return Visibility.Hidden;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
