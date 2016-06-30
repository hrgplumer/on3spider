using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpiderEngine.Models.Domain;

namespace SpiderEngine.Models.Result
{
    public class RosterResult
    {
        public IEnumerable<Player> Players { get; set; }
    }
}
