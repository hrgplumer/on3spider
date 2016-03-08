using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToExcel;
using On3Spider.Models;

namespace On3Spider.Infrastructure
{
    /// <summary>
    /// Class for reading an Excel 2007 spreadsheet using LinqToExcel
    /// </summary>
    public class ExcelReader
    {
        private readonly string _fileName;

        public ExcelReader(string fileName)
        {
            _fileName = fileName;
        }

        /// <summary>
        /// Reads rows from an Excel spreadsheet using the schema laid out in the ExcelUrl class.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> ReadUrls()
        {
            if (String.IsNullOrWhiteSpace(_fileName))
            {
                throw new InvalidOperationException("Attempted to read an invalid Excel file.");
            }

            var excel = new ExcelQueryFactory(_fileName);
            var urls = excel.Worksheet<ExcelUrl>().Select(u => u.Url);
            return urls;
        } 


    }
}
