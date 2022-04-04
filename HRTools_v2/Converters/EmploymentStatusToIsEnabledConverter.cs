using Domain.Types;
using HRTools_v2.Converters.Base;
using System;
using System.Globalization;

namespace HRTools_v2.Converters
{
    public class EmploymentStatusToIsEnabledConverter : BaseValueConverter<EmploymentStatusToIsEnabledConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = (EmploymentStatus)value;

            var param = parameter.ToString();

            switch(param)
            {
                // Activate Empl
                case "1":
                    return !state.Equals(EmploymentStatus.Active);

                // Suspend Empl
                case "2":
                    return !state.Equals(EmploymentStatus.Suspended);
                default:
                    return false;
            }

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
