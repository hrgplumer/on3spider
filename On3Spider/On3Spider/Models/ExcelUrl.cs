using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToExcel.Attributes;

namespace On3Spider.Models
{
    public class ExcelUrl
    {
        [ExcelColumn("URL")]
        public string Url { get; set; }
    }
}
