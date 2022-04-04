using HRTools_v2.Converters.Base;
using System;
using System.Globalization;

namespace HRTools_v2.Converters
{
    public class TenureToProbationStringConverter : BaseValueConverter<TenureToProbationStringConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime.TryParse(value.ToString(), out DateTime lastHireDate);

            return lastHireDate == null || lastHireDate.Equals(DateTime.MinValue)
                ? "unknown"
                : (DateTime.Now - lastHireDate).Days > 90 ? "Outside the Probation" : "On Probation";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
