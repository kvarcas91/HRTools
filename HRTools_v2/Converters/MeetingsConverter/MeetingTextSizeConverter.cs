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
            if (value == null) return Application.Current.FindResource("PrimaryListFontSize");

            switch (value)
            {
                case "Cancelled":
                case "Resigned":
                case "Terminated":
                    return Application.Current.FindResource("SecondaryListFontSize");
                default:
                    return Application.Current.FindResource("PrimaryListFontSize");
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
