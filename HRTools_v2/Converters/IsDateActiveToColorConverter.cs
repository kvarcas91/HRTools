using HRTools_v2.Converters.Base;
using System;
using System.Globalization;

namespace HRTools_v2.Converters
{
    public class IsDateActiveToColorConverter : BaseValueConverter<IsDateActiveToColorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime.TryParse(value.ToString(), out DateTime endDate);

            return endDate > DateTime.Now ? "Red" : "Gray";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
