using Domain.Types;
using HRTools_v2.Converters.Base;
using System;
using System.Globalization;
using System.Windows;

namespace HRTools_v2.Converters
{
    public class UIComponentStateToVisibilityConverter : BaseValueConverter<UIComponentStateToVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null) throw new NotImplementedException();

            var state = (UIComponentState)value;

            switch (parameter)
            {
                case "Loader":
                    return state.Equals(UIComponentState.Loading) ? Visibility.Visible : Visibility.Collapsed;
                case "RequireLoaded":
                    return state.Equals(UIComponentState.Visible) ? Visibility.Visible : Visibility.Collapsed;
                case "Empty":
                    return state.Equals(UIComponentState.Empty) ? Visibility.Visible : Visibility.Collapsed;
                default:
                    return Visibility.Collapsed;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
