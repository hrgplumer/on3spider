using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbotX.Parallel;

namespace SpiderEngine.Interface
{
    public interface ICrawler
    {
        void CrawlAsync();
        void StopCrawl(bool isHardStop);
        event EventHandler<AllCrawlsCompletedArgs> AllCrawlsCompleted;
        event EventHandler<SiteCrawlCompletedArgs> SiteCrawlCompleted;
        event EventHandler<CrawlerInstanceCreatedArgs> CrawlerInstanceCreated;
    }
}
