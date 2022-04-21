using HRTools_v2.Converters.Base;
using System;
using System.Globalization;
using System.Windows.Controls;

namespace HRTools_v2.Converters
{
    public class MeetingStatusSelectionToStringConverter : BaseValueConverter<MeetingStatusSelectionToStringConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as ComboBoxItem).Content.ToString();
        }
    }
}
