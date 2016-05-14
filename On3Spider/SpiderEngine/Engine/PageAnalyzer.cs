using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abot.Poco;
using SpiderEngine.Infrastructure;
using SpiderEngine.Interface;
using SpiderEngine.Models;

namespace SpiderEngine.Engine
{
    public class PageAnalyzer
    {
        private readonly IEnumerable<CrawledPage> _pages;
        private readonly String _category;
        private readonly Dictionary<string, ISheetRow> _urlDictionary; 

        public PageAnalyzer(IEnumerable<CrawledPage> pages, string category, Dictionary<string, ISheetRow> urlDict)
        {
            _pages = pages;
            _category = category;
            _urlDictionary = urlDict;
        }

        /// <summary>
        /// Kickoff method to analyze pages asyncronously
        /// </summary>
        /// <returns></returns>
        public async Task<IList<AnalyzeResult>> AnalyzeAsync()
        {
            if (_pages == null || !_pages.Any())
            {
                throw new InvalidOperationException("Cannot begin analyzing null pages. Set pages in constructor.");
            }

            return await Task.Run(() => AnalyzePages());
        }

        /// <summary>
        /// Magic page analyzing goes in here
        /// </summary>
        /// <returns></returns>
        private IList<AnalyzeResult> AnalyzePages()
        {
            Trace.WriteLine($"begin analyzing {_pages.Count()} pages in new thread");

            var results = new List<AnalyzeResult>();

            switch (_category)
            {
                case Constants.FileCategory.Roster:
                    results = AnalyzeRosterUrls();
                    break;
            }

            Trace.WriteLine("returning from analyzer thread");
            return results;
        }

        private List<AnalyzeResult> AnalyzeRosterUrls()
        {
            return null;
        } 
    }
}
