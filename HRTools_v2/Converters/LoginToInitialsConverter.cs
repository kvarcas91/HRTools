using Domain.Storage;
using HRTools_v2.Converters.Base;
using System;
using System.Globalization;
using System.Linq;

namespace HRTools_v2.Converters
{
    public class LoginToInitialsConverter : BaseValueConverter<LoginToInitialsConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            if (value.ToString().Contains("automation")) return "TA";

            var name = DataStorage.RosterList.Where(x => x.UserID == value.ToString()).Select(n => n.EmployeeName).FirstOrDefault();
            if (name != null)
            {
                name = name.ToUpper();
                var splt = name.Split(',');
                if (splt.Length > 1) return $"{splt[1][0]}{splt[0][0]}";

                if (name.Length > 1) name.Substring(0, 2);
                return name[0];
            }

            return char.ToUpper(value.ToString()[0]);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
