using HRTools_v2.Converters.Base;
using System;
using System.Globalization;

namespace HRTools_v2.Converters
{
    public class UserToTextConverter : BaseValueConverter<UserToTextConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";
            if (string.IsNullOrEmpty(value.ToString())) return "";

            switch(parameter)
            {
                case "created":
                    return $"Created by {value} ";
                case "updated":
                    return $"Last updated by {value} ";
                case "bridged":
                    return $"Bridge created by {value} ";
                default:
                    return "";
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
