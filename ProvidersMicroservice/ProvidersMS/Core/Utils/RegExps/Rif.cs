using System.Text.RegularExpressions;

namespace ProvidersMS.Core.Utils.RegExps
{
    public class RifRegex
    {
        public static readonly Regex RifRegexs = new Regex(
        @"^[JGVEP][0-9]{9}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        internal static bool IsMatch(string rif)
        {
            return RifRegexs.IsMatch(rif);
        }
    }
}
