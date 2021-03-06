using Domain.Storage;
using HRTools_v2.Converters.Base;
using System;
using System.Globalization;
using System.Linq;

namespace HRTools_v2.Converters
{
    public class LoginToFullNameConverter : BaseValueConverter<LoginToFullNameConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString())) return value;
           var name = DataStorage.RosterList.Where(x => x.UserID == value.ToString().Trim()).Select(n => n.EmployeeName).FirstOrDefault();
            if (name != null)
            {
                var splt = name.Split(',');
                if (splt.Length > 1) return $"{splt[1]} {splt[0]}";

                return name;
            }

            return value;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
