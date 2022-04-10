using Domain.Types;
using HRTools_v2.Converters.Base;
using System;
using System.Globalization;

namespace HRTools_v2.Converters
{
    public class AwalTextDecorationConverter : BaseValueConverter<AwalTextDecorationConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var awalStatus = (AwalStatus)value;
                switch (awalStatus)
                {
                    case AwalStatus.Cancelled:
                        return "Strikethrough";
                    default:
                        return "None";
                }
            }
            catch
            {
                return "None";
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
