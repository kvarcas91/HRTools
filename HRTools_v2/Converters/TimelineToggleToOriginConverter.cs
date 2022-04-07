using Domain.Types;
using HRTools_v2.Converters.Base;
using System;
using System.Globalization;

namespace HRTools_v2.Converters
{
    public class TimelineToggleToOriginConverter : BaseValueConverter<TimelineToggleToOriginConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var origin = (TimelineOrigin)value;
            Enum.TryParse(parameter.ToString(), out TimelineOrigin selection);
           

            return origin == selection;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isSelected = System.Convert.ToBoolean(value);
            Enum.TryParse(parameter.ToString(), out TimelineOrigin selection);

            return isSelected ? selection : TimelineOrigin.ALL;
        }
    }
}
