﻿using System;
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
using SpiderEngine.Models.Domain;
using SpiderEngine.Models.Result;
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
        public async Task<IList<ISheetRow>> AnalyzeAsync()
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
        private IList<ISheetRow> AnalyzePages()
        {
            Trace.WriteLine($"begin analyzing {_pages.Count()} pages in new thread");

            var results = new List<ISheetRow>();

            switch (_category)
            {
                case Constants.FileCategory.Roster:
                    results = AnalyzeRosterUrls();
                    break;
            }

            Trace.WriteLine("returning from analyzer thread");
            return results;
        }

        private List<ISheetRow> AnalyzeRosterUrls()
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

                var playerList = new List<Player>();
                foreach (var table in tables)
                {
                    // look for table header. if not found, use first row
                    var tHead = table.SelectSingleNode("//thead");
                    var firstRow = tHead != null ? tHead.SelectSingleNode("./tr") : table.SelectSingleNode(".//tr");

                    // Couldn't get a first row; just discard this table
                    if (firstRow == null)
                    {
                        continue;
                    }

                    // get cells of first row. determine column names from this row
                    var headerCells = firstRow.SelectNodes("./th | ./td");

                    // look for player number column
                    var playerNumberCell = headerCells.FirstOrDefault(cell => RegexRepository.NumberColumnRegex.IsMatch(cell.InnerText));
                    // get index of this column
                    var playerNumberCellIndex = headerCells.IndexOf(playerNumberCell);

                    // Look for a full name column; if not found, check next table
                    if (!headerCells.Skip(playerNumberCellIndex + 1).Any(n => RegexRepository.NameColumnRegex.IsMatch(n.InnerText)))
                    {
                        continue;
                    }

                    var playerNameIndex =
                        headerCells.IndexOf(headerCells.FirstOrDefault(n => RegexRepository.NameColumnRegex.IsMatch(n.InnerText)));
                    var numColsInTable = headerCells.Count;

                    // If there was a <thead> hopefully there's a <tbody>
                    // If so, capture all the rows in <tbody>. Otherwise,
                    // get all rows in this table except the first one

                    HtmlNodeCollection dataRows;
                    var tableBody = table.SelectSingleNode("//tbody");
                    if (tableBody != null)
                    {
                        dataRows = tableBody.SelectNodes(".//tr");
                    }
                    else
                    {
                        dataRows = table.SelectNodes(".//tr[position()>=1]");
                    }

                    // iterate over each row, skipping rows with different numbers
                    // of columns than in the header row. For each matching row,
                    // get the player name and player URL from the appropriate
                    // columns.

                    foreach (var row in dataRows)
                    {
                        var rowCells = row.SelectNodes("./td");
                        if (rowCells == null || rowCells.Count != numColsInTable)
                        {
                            continue;
                        }

                        var playerCell = rowCells[playerNameIndex];
                        var playerUrlTag = playerCell.SelectSingleNode("a") ?? playerCell.SelectSingleNode(".//a");
                        var playerName = playerUrlTag.InnerText.Trim();

                        // check if name is listed LAST, FIRST and if so, reverse it
                        if (playerName.Contains(","))
                        {
                            var nameArray = playerName.Split(',');
                            if (nameArray.Length == 2) // if length isn't two, just leave the name alone
                            {
                                playerName = String.Empty;
                                playerName += nameArray[1] + " ";
                                playerName += nameArray[0];
                            }
                        }

                        // find out more info about player here (position? height/weight? class?)

                        /* **********

                        */

                        playerList.Add(new Player()
                        {
                            FirstLast = playerName
                        });
                    }
                }

                // find this Url in the dictionary and add the list of player results
                if (_urlDictionary.ContainsKey(page.Uri.ToString()))
                {
                    var pageRosterSheet = _urlDictionary[page.Uri.ToString()];
                    pageRosterSheet.Results = new RosterResult()
                    {
                        Players = playerList
                    };
                }
                else
                {
                    //throw new InvalidOperationException("On page we aren't supposed to be on. " + page.Uri.ToString());
                    // We are on a URL not in the dictionary. skip
                    continue;
                }

            }

            // figure out a way to return this stuff
            return _urlDictionary.Values.ToList();
        }
    }
}
