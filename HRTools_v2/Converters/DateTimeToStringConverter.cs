using HRTools_v2.Converters.Base;
using System;
using System.Globalization;
using System.Windows.Data;

namespace HRTools_v2.Converters
{
    public class DateTimeToStringConverter : BaseValueConverter<DateTimeToStringConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return "";
            try
            {
                DateTime dateTime = DateTime.Parse(value.ToString());

                if (dateTime.Equals(DateTime.MinValue) && parameter.Equals("1")) return "-";

                if (dateTime.Equals(DateTime.MinValue)) return "";

                if (parameter != null && parameter.Equals("3"))
                {
                    return dateTime.ToString("HH:mm");
                }

                if (parameter != null && parameter.Equals("2"))
                {
                    return dateTime.ToString("dd MMM, yyyy");
                }

                // Ignore difference between days if true
                if (parameter != null)
                    return dateTime.ToString("dd/MM/yyyy");

                if (dateTime.Date == DateTime.UtcNow.Date)
                    //return just time
                    return dateTime.ToString("HH:mm");

                // if it is not today..
                return dateTime.ToString("HH:mm, dd MMM, yyyy");
            }
            catch
            {
                return "-";
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
