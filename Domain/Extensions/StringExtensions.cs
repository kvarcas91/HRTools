using Domain.Storage;
using System;

namespace Domain.Extensions
{
    public static class StringExtensions
    {
        public static string DbSanityCheck(this string query)
        {
            return query?.Replace("'", "''");
        }

        public static bool IsValidDigitID(this string str)
        {
            foreach (var c in str.Trim())
            {
                if (!char.IsDigit(c)) return false;
            }

            return true;
        }

        public static string VerifyCSV(this string data)
        {
            if (string.IsNullOrEmpty(data)) return string.Empty;

            if (data.Contains(","))
            {
                if (!data.StartsWith("\"") || !data.EndsWith("\""))
                {
                    data = data.Replace("\"", "");
                    data = string.Format("\"{0}\"", data);
                }
                    
            }

            if (data.Contains(Environment.NewLine))
            {
                data = string.Format("\"{0}\"", data);
            }

            if (data.Equals("Default") || data.Equals("NoOutcome")) data = string.Empty;

            if (data.Equals(DateTime.MinValue.ToString(DataStorage.LongPreviewDateFormat)) || data.Equals(DateTime.MinValue.ToString(DataStorage.ShortPreviewDateFormat))) data = string.Empty;

            return data;
        }
    }
}
