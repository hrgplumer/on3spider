using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Abot.Crawler;
using Abot.Poco;
using Abot.Util;
using AbotX.Crawler;
using AbotX.Parallel;
using AbotX.Poco;
using log4net;
using log4net.Config;

namespace SpiderEngine.Engine
{
    /// <summary>
    /// Wrapper class for AbotX ParallelCrawlerEngine that handles configuration, startup, shutdown
    /// </summary>
    public class Crawler
    {
        private readonly ParallelCrawlerEngine _crawler;

        /// <summary>
        /// Initializes the AbotX parallel crawler using app.config settings, to crawl the list of urls provided.
        /// </summary>
        /// <param name="urls">The list of urls the Crawler should visit.</param>
        public Crawler(IEnumerable<string> urls /* inject logger, output class here */)
        {
            // configure log4net -- this must be called before crawler is created
            XmlConfigurator.Configure();

            var provider = new SiteToCrawlProvider();
            provider.AddSitesToCrawl(urls.Select(url => 
            new SiteToCrawl()
            {
                Uri = new Uri(url)
            }));

            _crawler = new ParallelCrawlerEngine(provider);

            //Register for site level events
            _crawler.AllCrawlsCompleted += (sender, eventArgs) =>
            {
                Trace.WriteLine("Completed crawling all sites");
            };
            _crawler.SiteCrawlCompleted += (sender, eventArgs) =>
            {
                Trace.WriteLine(String.Format("Completed crawling site {0}", eventArgs.CrawledSite.SiteToCrawl.Uri));
            };
            _crawler.CrawlerInstanceCreated += (sender, eventArgs) =>
            {
                //Register for crawler level events. These are Abot's events!!!
                eventArgs.Crawler.PageCrawlCompleted += (abotSender, abotEventArgs) =>
                {
                    CrawledPage crawledPage = abotEventArgs.CrawledPage;

                    if (crawledPage.WebException != null || crawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)
                        Trace.WriteLine(String.Format("Crawl of page failed {0}", crawledPage.Uri.AbsoluteUri));
                    else
                        Trace.WriteLine(String.Format("Crawl of page succeeded {0}", crawledPage.Uri.AbsoluteUri));

                    if (string.IsNullOrEmpty(crawledPage.Content.Text))
                        Trace.WriteLine(String.Format("Page had no content {0}", crawledPage.Uri.AbsoluteUri));
                };
            };
        }

        /// <summary>
        /// Begins crawling pages asynchronously.
        /// </summary>
        public void CrawlAsync()
        {
            _crawler.StartAsync();
        }

        /// <summary>
        /// Stops the crawl.
        /// </summary>
        /// <param name="isHardStop">(Optional) Determins whether this crawl stop is a hard stop. 
        ///  A hard stop will stop immediately, a soft one will wait for any current crawls to finish.</param>
        public void StopCrawl(bool isHardStop = false)
        {
            _crawler.Stop(isHardStop);
        }
    }
}
