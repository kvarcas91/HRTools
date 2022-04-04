using Domain.Types;
using HRTools_v2.Converters.Base;
using System;
using System.Globalization;

namespace HRTools_v2.Converters
{
    public class EmploymentStatusToColorConverter : BaseValueConverter<EmploymentStatusToColorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = (EmploymentStatus)value;

            switch (state)
            {
                case EmploymentStatus.Active:
                    return "#4caf50";
                case EmploymentStatus.Suspended:
                    return "#ffc107";
                case EmploymentStatus.NotActive:
                    return "#ef5350";
                default:
                    return "transparent";
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
