using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abot.Poco;
using SpiderEngine.Models;

namespace SpiderEngine.Engine
{
    public class PageAnalyzer
    {
        private IEnumerable<CrawledPage> _pages; 

        public PageAnalyzer(IEnumerable<CrawledPage> pages )
        {
            _pages = pages;
        }

        public async Task<IList<AnalyzeResult>> AnalyzeAsync()
        {
            if (_pages == null || !_pages.Any())
            {
                throw new InvalidOperationException("Cannot begin analyzing null pages. Set pages in constructor.");
            }

            return await Task.Run(() => AnalyzePages());
        }

        private IList<AnalyzeResult> AnalyzePages()
        {
            Trace.WriteLine($"begin analyzing {_pages.Count()} pages in new thread");


            Trace.WriteLine("returning from analyzer thread");
            return new List<AnalyzeResult>();
        } 
    }
}
