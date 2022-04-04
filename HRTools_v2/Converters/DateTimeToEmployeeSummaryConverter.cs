using HRTools_v2.Converters.Base;
using System;
using System.Globalization;

namespace HRTools_v2.Converters
{
    public class DateTimeToEmployeeSummaryConverter : BaseValueConverter<DateTimeToEmployeeSummaryConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                DateTime date = DateTime.Parse(value.ToString());
                if (parameter.Equals("day"))
                {
                    return date.Day.ToString();
                }
                else
                {
                    return date.ToString("MMM, yyyy");
                }
            }
            catch
            {
                return value;
            }

        }
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
