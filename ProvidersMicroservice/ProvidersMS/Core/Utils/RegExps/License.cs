using System.Text.RegularExpressions;

namespace ProvidersMS.Core.Utils.RegExps
{
    public class LicenseRegex
    {
        private static readonly Regex LicenseRegexs = new Regex(
            @"^[A-Z0-9]{1,15}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        internal static bool IsMatch(string license)
        {
            return LicenseRegexs.IsMatch(license);
        }
    }
}
