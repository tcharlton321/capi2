using System.Globalization;

namespace CaesarsAPI.Services
{
    public static class CaesarUtils
    {
        public static DateTime? getDate(string input)
        {
            var formats = new[] { "M/d/yyyy", "M/dd/yyyy", "MM/d/yyyy", "MM/dd/yyyy", "M-d-yyyy", "M-dd-yyyy", "MM-d-yyyy", "MM-dd-yyyy", "yyyy/M/d", "yyyy/M/dd", "yyyy/MM/d/", "yyyy/MM/dd", "yyyy-M-d", "yyyy-M-dd", "yyyy-MM-d", "yyyy-MM-dd" };
            DateTime dt;
            if (DateTime.TryParseExact(input, formats, null, DateTimeStyles.None, out dt))
            {
                Console.WriteLine(dt.ToString());
                return dt;
            }

            return null;
        }

        public static string Salt()
        {
            return "-0Always1Salt2Your3Passwords4";
        }
    }
}
