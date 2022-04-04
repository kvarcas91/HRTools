using HRTools_v2.Converters.Base;
using System;
using System.Globalization;

namespace HRTools_v2.Converters
{
    public class SanctionTextDecorationConverter : BaseValueConverter<SanctionTextDecorationConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                DateTime.TryParse(value.ToString(), out DateTime endDate);

                return endDate <= DateTime.Now ? "Strikethrough" : "None";
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
