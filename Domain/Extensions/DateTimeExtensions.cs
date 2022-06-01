using System;

namespace Domain.Extensions
{
    public static class DateTimeExtensions
    {
        public static string DbSanityCheck(this DateTime date, string f)
        {
            return date.Equals(DateTime.MinValue) ? null : date.ToString(f);
        }
        public static string DbNullableSanityCheck(this DateTime date, string f)
        {
            return date.Equals(DateTime.MinValue) ? "NULL" : $"'{date.ToString(f)}'";
        }


    }
}
