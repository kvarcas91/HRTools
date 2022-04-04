using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Domain.Storage;
using System;

namespace Domain.Converters
{
    public class DateConverter<T> : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {

            DateTime dateTime;

            try
            {
                DateTime.TryParseExact(text, DataStorage.AppSettings.RosterWebDateFormat, null, System.Globalization.DateTimeStyles.None, out dateTime);
                return dateTime;
            }
            catch
            {
                return base.ConvertFromString(text, row, memberMapData);
            }
        }

        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            return base.ConvertToString(value, row, memberMapData);
        }
    }
}
