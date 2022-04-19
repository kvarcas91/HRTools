using HRTools_v2.Converters.Base;
using System;
using System.Globalization;

namespace HRTools_v2.Converters
{
    public class MeetingTextDecorationConverter : BaseValueConverter<MeetingTextDecorationConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value == null) return "None";

            switch (value)
            {
                case "Cancelled":
                case "Resigned":
                case "Terminated":
                    return "Strikethrough";
                default:
                    return "None";
            }
           
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
