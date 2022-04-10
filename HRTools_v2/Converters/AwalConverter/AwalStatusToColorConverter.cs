using Domain.Types;
using HRTools_v2.Converters.Base;
using System;
using System.Globalization;

namespace HRTools_v2.Converters
{
    public class AwalStatusToColorConverter : BaseValueConverter<AwalStatusToColorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var awalStatus = (AwalStatus)value;
            switch (awalStatus)
            {
                case AwalStatus.Active:
                    return "Red";
                case AwalStatus.Pending:
                    return "Orange";
                    default:
                    return "Gray";
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
