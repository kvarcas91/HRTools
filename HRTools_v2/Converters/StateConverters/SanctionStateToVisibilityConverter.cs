using Domain.Types;
using HRTools_v2.Converters.Base;
using System;
using System.Globalization;
using System.Windows;

namespace HRTools_v2.Converters
{
    public class SanctionStateToVisibilityConverter : BaseValueConverter<SanctionStateToVisibilityConverter>
    {
        

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null) throw new NotImplementedException();

            var state = (SanctionWidgetState)value;
            Enum.TryParse(parameter.ToString(), out SanctionWidgetState widget);

            var hasFlag = (state & widget) != 0;

            return hasFlag ? Visibility.Visible : Visibility.Collapsed;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
