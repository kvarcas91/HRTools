namespace Domain.Extensions
{
    public static class StringExtensions
    {
        public static string DbSanityCheck(this string query)
        {
            return query?.Replace("'", "''");
        }
    }
}
