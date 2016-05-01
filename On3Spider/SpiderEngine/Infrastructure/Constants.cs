using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderEngine.Infrastructure
{
    public static class Constants
    {
        public static class FileCategory
        {
            const string Roster = "Roster";
            const string Schedule = "Schedule";
            const string Homepage = "Homepage";
            public const string DefaultValue = "Set Category";

            public static IList<String> FileCategoryItems => new List<string>
            {
                Roster,
                Schedule,
                Homepage
            };
        }
    }
}
