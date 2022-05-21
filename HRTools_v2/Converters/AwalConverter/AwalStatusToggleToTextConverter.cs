using HRTools_v2.Converters.Base;
using System;
using System.Globalization;

namespace HRTools_v2.Converters
{
    public class AwalStatusToggleToTextConverter : BaseValueConverter<AwalStatusToggleToTextConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            var toggle = System.Convert.ToBoolean(value);
            return toggle ? "Show all cases" : "Show open cases only";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
