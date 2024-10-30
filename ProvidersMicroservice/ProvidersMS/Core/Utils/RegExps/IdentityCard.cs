using System.Text.RegularExpressions;

namespace ProvidersMS.Core.Utils.RegExps
{
    public class IdentityCardRegex
    {
        public static readonly Regex IdentityCardRegexs = new Regex(
        @"^[0-9]{9}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        internal static bool IsMatch(string identity)
        {
            return IdentityCardRegexs.IsMatch(identity);
        }
    }
}
