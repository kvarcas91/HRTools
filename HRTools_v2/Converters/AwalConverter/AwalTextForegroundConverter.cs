﻿using Domain.Types;
using HRTools_v2.Converters.Base;
using System;
using System.Globalization;

namespace HRTools_v2.Converters
{
    public class AwalTextForegroundConverter : BaseValueConverter<AwalTextForegroundConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var awalStatus = (AwalStatus)value;

                return awalStatus.Equals(AwalStatus.Active) ? "Black" : "Gray";
            }
            catch
            {
                return "Gray";
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}