using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpiderEngine.Repository
{
    /// <summary>
    /// Contains Regex objects needed for parsing Html
    /// </summary>
    public static class RegexRepository
    {
        /// <summary>
        /// Regex used to determine if a column contains player number.
        /// </summary>
        /// <returns></returns>
        public static Regex NumberColumnRegex()
        {
            return new Regex(@"\s*(?:No.?|#)\s*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public static Regex NameColumnRegex()
        {
            return new Regex(@"^\s*(Full\s+)?Name\s*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }
    }
}
