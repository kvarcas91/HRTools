using Domain.Types;
using HRTools_v2.Converters.Base;
using System;
using System.Globalization;
using System.Windows;

namespace HRTools_v2.Converters
{
    public class AwalTextSizeConverter : BaseValueConverter<AwalTextSizeConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var awalStatus = (AwalStatus)value;
                switch (awalStatus)
                {
                    case AwalStatus.Cancelled:
                    case AwalStatus.Resigned:
                        return Application.Current.FindResource("SecondaryListFontSize");
                    default:
                        return Application.Current.FindResource("PrimaryListFontSize");
                }
            }
            catch
            {
                return Application.Current.FindResource("SecondaryListFontSize");
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
