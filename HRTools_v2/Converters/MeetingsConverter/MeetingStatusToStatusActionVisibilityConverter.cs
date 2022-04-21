using HRTools_v2.Converters.Base;
using System;
using System.Globalization;
using System.Windows;

namespace HRTools_v2.Converters
{
    public class MeetingStatusToStatusActionVisibilityConverter : BaseValueConverter<MeetingStatusToStatusActionVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value.ToString())
            {
                case "Open":
                    return parameter.ToString() == "pending" ? Visibility.Visible : Visibility.Hidden;
                case "Pending":
                    return parameter.ToString() == "pending" ? Visibility.Hidden : Visibility.Visible;
                default:
                    return parameter.ToString() == "pending" ? Visibility.Hidden : Visibility.Visible;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
