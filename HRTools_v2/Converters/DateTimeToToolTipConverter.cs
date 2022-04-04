using Domain.Storage;
using HRTools_v2.Converters.Base;
using System;
using System.Globalization;

namespace HRTools_v2.Converters
{
    public class DateTimeToToolTipConverter : BaseValueConverter<DateTimeToToolTipConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value.ToString())) return "";
            try
            {
                var date = DateTime.Parse(value.ToString());
                if (date.Equals(DateTime.MinValue)) return "";
                return $"on {date.ToString(DataStorage.LongPreviewDateFormat)}";
            }
            catch { return ""; }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
