using HRTools_v2.Converters.Base;
using System;
using System.Globalization;

namespace HRTools_v2.Converters
{
    public class MeetingOutcomeToNextStageConverter : BaseValueConverter<MeetingOutcomeToNextStageConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
             return !(value != null & (value.Equals("NFA") || value.Equals("Cancelled")));
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
