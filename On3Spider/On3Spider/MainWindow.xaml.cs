using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Abot.Poco;
using Microsoft.Win32;
using On3Spider.Infrastructure;
using SpiderEngine.Engine;

namespace On3Spider
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            //var openFileDialog = new OpenFileDialog()
            //{
            //    Filter = "Excel files (*.xlsx)|*.xlsx"
            //};

            //if (openFileDialog.ShowDialog() != true) return;

            var fileName = //openFileDialog.FileName;
                @"E:\j\work\personal\projects\on3\test sheets\simple_test_3_urls.xlsx";

            var reader = new ExcelReader(fileName);
            var urls = reader.ReadUrls().ToList();

            if (!urls.Any())
            {
                return;
            }

            StartCrawlingEngine(urls);
        }

        /// <summary>
        /// Starts the crawling engine.
        /// </summary>
        /// <param name="urls">The list of urls to crawl.</param>
        private void StartCrawlingEngine(IEnumerable<string> urls)
        {
            var manager = new EngineManager(new Crawler(urls), new QueueManager<CrawledPage>());
            manager.Start();
        }
    }
}
