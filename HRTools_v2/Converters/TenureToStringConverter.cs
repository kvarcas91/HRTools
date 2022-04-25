using HRTools_v2.Converters.Base;
using System;
using System.Globalization;

namespace HRTools_v2.Converters
{
    public class TenureToStringConverter : BaseValueConverter<TenureToStringConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime.TryParse(value.ToString(), out DateTime lastHireDate);

            if (lastHireDate == null || lastHireDate.Equals(DateTime.MinValue)) return "unknown";

            DateTime zeroTime = new DateTime(1, 1, 1);
            TimeSpan span = DateTime.Now - lastHireDate;

            int years = (zeroTime + span).Year - 1;
            int months = (zeroTime + span).Month - 1;
            int days = (zeroTime + span).Day - 1;

            var yearText = years > 0 ? years > 1 ? $"{years} years" : $"{years} year" : "";
            var yearMonthSeparator = years > 0 ? " " : "";
            var monthText = months > 0 ? months > 1 ? $"{months} months" : $"{months} month" : "";
            var monthDaySeparator = months > 0 ? " " : "";
            var daysText = days > 0 ? days > 1 ? $"{days} days" : $"{days} day" : "";

            if (years + months + days == 0) return "Created today";

            return $"{yearText}{yearMonthSeparator}{monthText}{monthDaySeparator}{daysText}";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
