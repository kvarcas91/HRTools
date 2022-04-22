using HRTools_v2.Converters.Base;
using System;
using System.Globalization;

namespace HRTools_v2.Converters
{
    public class MeetingTextForegroundConverter : BaseValueConverter<MeetingTextForegroundConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "Black";

            switch (value)
            {
                case "Cancelled":
                case "Resigned":
                case "Terminated":
                case "Closed":
                    return "Gray";
                default:
                    return "Black";
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
