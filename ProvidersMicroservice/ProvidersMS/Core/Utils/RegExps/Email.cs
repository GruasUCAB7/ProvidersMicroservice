using System.Text.RegularExpressions;

namespace ProvidersMS.Core.Utils.RegExps
{
    public class EmailRegex
    {
        public static readonly Regex EmailRegexs = new Regex(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

        internal static bool IsMatch(string email)
        {
            return EmailRegexs.IsMatch(email);
        }
    }
}
