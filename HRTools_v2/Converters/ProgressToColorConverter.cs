using HRTools_v2.Converters.Base;
using System;
using System.Globalization;

namespace HRTools_v2.Converters
{
    public class ProgressToColorConverter : BaseValueConverter<ProgressToColorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            var color = "White";

            try
            {

                switch ((decimal)value)
                {
                    case decimal n when n >= 100:
                        color = "#2196F3";
                        break;
                    case decimal n when n < 100 && n >= 75:
                        color = "#5971B6";
                        break;
                    case decimal n when n < 75 && n >= 50:
                        color = "#904B7A";
                        break;
                    case decimal n when n < 50 && n >= 25:
                        color = "#C8263D";
                        break;
                    case decimal n when n < 25:
                        color = "#FF0000";
                        break;
                    default:
                        break;
                }

                return color;
            }
            catch (Exception e)
            {
                return color;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
