using HRTools_v2.Converters.Base;
using System;
using System.Globalization;

namespace HRTools_v2.Converters
{
    public class ProgressConverter : BaseValueConverter<ProgressConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var progress = System.Convert.ToDecimal(value);

            // Ignore difference between days if true

            if (progress == 100) return 99.9D;
            else return progress;

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
