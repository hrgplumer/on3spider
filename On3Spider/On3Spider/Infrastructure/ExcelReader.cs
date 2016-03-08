using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToExcel;

namespace On3Spider.Infrastructure
{
    public class ExcelReader
    {
        private readonly string _fileName;

        public ExcelReader(string fileName)
        {
            _fileName = fileName;
        }

        public IEnumerable<string> ReadUrls()
        {
            if (String.IsNullOrWhiteSpace(_fileName))
            {
                throw new InvalidOperationException("Attempted to read an invalid Excel file.");
            }

            var excel = new ExcelQueryFactory(_fileName);
            return null;
        } 


    }
}
