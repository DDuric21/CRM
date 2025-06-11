using System.Globalization;

namespace UI.Helpers
{
    public static class ParsingExtensions
    {
        public static string ToStringValue(this DateTime date)
        {
            if (date == DateTime.MinValue)
            {
                return string.Empty;
            }

            return date.ToString("dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}
