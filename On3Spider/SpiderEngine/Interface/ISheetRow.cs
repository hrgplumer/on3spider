using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpiderEngine.Models.Domain;
using SpiderEngine.Models.Result;

namespace SpiderEngine.Interface
{
    public interface ISheetRow
    {
        string School { get; set; }
        string Sport { get; set; }
        string Url { get; set; }
        RosterResult Results { get; set; } 
    }
}
