using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Abot.Crawler;
using Abot.Poco;
using AbotX.Parallel;
using SpiderEngine.Interface;

namespace SpiderEngine.Engine
{
    /// <summary>
    /// Manager class for all SpiderEngine tasks.
    /// </summary>
    public class EngineManager
    {
        private readonly ICrawler _crawler;
        private readonly IQueueManager<CrawledPage> _queue;

        private bool _allCrawlsCompleted = false;

        /// <summary>
        /// Create a new EngineManager using the given crawler and queue.
        /// </summary>
        /// <param name="crawler">An ICrawler instance. This will be used as the web crawler.</param>
        /// <param name="queue">An IQueueManager instance. This will be used as the thread safe queue for processing crawled pages.</param>
        public EngineManager(ICrawler crawler, IQueueManager<CrawledPage> queue)
        {
            if (crawler == null)
                throw new ArgumentNullException(nameof(crawler));
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

            _crawler = crawler;
            _queue = queue;

            RegisterCrawlerEvents();
        }

        /// <summary>
        /// Starts the web crawl.
        /// </summary>
        public async Task StartAsync()
        {
            _crawler.Crawl();
            using (var processTask = ProcessCrawledPagesAsync())
            {
                await processTask;
            }
        }

        private async Task ProcessCrawledPagesAsync()
        {
            while (!_allCrawlsCompleted)
            {
                CrawledPage page = null;
                if (_queue.TryDequeue(out page))
                {
                    if (page == null) continue;

                    Trace.WriteLine($"Dequeued page {page.Uri.AbsoluteUri}: queue has {_queue.Count()} elements remaining");
                    // add a method here to process the page, and await it.
                    await ProcessPage(page);
                }
            }
        }

        private async Task ProcessPage(CrawledPage page)
        {
            var analyzer = new PageAnalyzer(page);
            Thread.Sleep(500);
        }

        /// <summary>
        /// Stops the web crawl.
        /// <param name="isHardStop">Determines whether the stop is a hard stop (immediately stops all threads)
        ///  or soft stop (waits for current processing to end, then stops)</param>
        /// </summary>
        public void Stop(bool isHardStop)
        {
            _crawler.StopCrawl(isHardStop);
        }

        #region Event Handlers

        public void Crawler_AllCrawlsCompleted(object sender, AllCrawlsCompletedArgs args)
        {
            _allCrawlsCompleted = true;
            Trace.WriteLine("Completed crawling all sites");
        }

        public void Crawler_SiteCrawlCompleted(object sender, SiteCrawlCompletedArgs args)
        {
            Trace.WriteLine($"Completed crawling site {args.CrawledSite.SiteToCrawl.Uri}");
        }

        public void Crawler_CrawlerInstanceCreated(object sender, CrawlerInstanceCreatedArgs args)
        {
            // register for crawler level events. this is an abot event
            args.Crawler.PageCrawlCompleted += Crawler_PageCrawlCompleted;
        }

        public async void Crawler_PageCrawlCompleted(object sender, PageCrawlCompletedArgs args)
        {
            var crawledPage = args.CrawledPage;

            if (crawledPage.WebException != null || crawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)
            {
                // an exception of some sort occured while crawling
                Trace.WriteLine($"Crawl of page failed {crawledPage.Uri.AbsoluteUri}");
            }
            else
            {
                Trace.WriteLine($"Crawl of page succeeded {crawledPage.Uri.AbsoluteUri}");

                // add crawled page to queue
                _queue.Enqueue(crawledPage);
                Trace.WriteLine($"Queue contains {_queue.Count()} items");
            }

            if (string.IsNullOrEmpty(crawledPage.Content.Text))
            {
                Trace.WriteLine($"Page had no content {crawledPage.Uri.AbsoluteUri}");
            }
        }

        #endregion

        #region Privates

        private void RegisterCrawlerEvents()
        {
            _crawler.AllCrawlsCompleted += Crawler_AllCrawlsCompleted;
            _crawler.SiteCrawlCompleted += Crawler_SiteCrawlCompleted;
            _crawler.CrawlerInstanceCreated += Crawler_CrawlerInstanceCreated;
        }

        #endregion
    }
}
