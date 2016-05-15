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
using HtmlAgilityPack;
using SpiderEngine.Repository;

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

            foreach (var page in _pages)
            {
                var html = page.HtmlDocument;

                // assume the roster will be in a table. grab all tables
                var tables = html.DocumentNode.SelectNodes("//table");
                if (tables == null)
                {
                    continue; // move to next page
                }

                foreach (var table in tables)
                {
                    // look for table header. if not found, use first row
                    var tHead = table.SelectSingleNode("//thead");
                    var firstRow = tHead != null ? tHead.SelectSingleNode("./tr") : table.SelectSingleNode(".//tr");

                    // get cells of first row. determine column names from this row
                    var headerCells = firstRow.SelectNodes("./th | ./td");

                    // look for player number column
                    var playerNumRegex = RegexRepository.NumberColumnRegex();
                    var playerNumberCell = headerCells.FirstOrDefault(cell => playerNumRegex.IsMatch(cell.InnerText));
                    // get index of this column
                    var playerNumberCellIdx = headerCells.IndexOf(playerNumberCell);


                }

            }

            return null;
        }
    }
}
