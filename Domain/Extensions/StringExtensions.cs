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
                if (!char.IsLetter(c)) return false;
            }

            return true;
        }
    }
}
