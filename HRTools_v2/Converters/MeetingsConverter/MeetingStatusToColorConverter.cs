using HRTools_v2.Converters.Base;
using System;
using System.Globalization;

namespace HRTools_v2.Converters
{
    public class MeetingStatusToColorConverter : BaseValueConverter<MeetingStatusToColorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "Gray";

            switch (value)
            {
                case "Open":
                    return "Red";
                case "Pending":
                    return "Orange";
                default:
                    return "Gray";
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
