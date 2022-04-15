using HRTools_v2.Converters.Base;
using System;
using System.Globalization;
using System.Windows;

namespace HRTools_v2.Converters
{
    public class MeetingTextSizeConverter : BaseValueConverter<MeetingTextSizeConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Application.Current.FindResource("PrimaryListFontSize") : value.Equals("Cancelled") ? 
                Application.Current.FindResource("SecondaryListFontSize") : Application.Current.FindResource("PrimaryListFontSize");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
