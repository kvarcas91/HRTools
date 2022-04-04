using HRTools_v2.Converters.Base;
using System;
using System.Globalization;
using System.Windows;

namespace HRTools_v2.Converters
{
    public class SanctionTextSizeConverter : BaseValueConverter<SanctionTextSizeConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                DateTime.TryParse(value.ToString(), out DateTime endDate);

                return endDate <= DateTime.Now ? Application.Current.FindResource("SecondaryListFontSize") : Application.Current.FindResource("PrimaryListFontSize");
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
