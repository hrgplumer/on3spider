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
using CsQuery.ExtensionMethods.Internal;
using Microsoft.Win32;
using On3Spider.Infrastructure;
using On3Spider.Models;
using SpiderEngine.Engine;
using SpiderEngine.Infrastructure;
using Path = System.IO.Path;

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
            InitializeCategoryComboBox();
        }

        private void InitializeCategoryComboBox()
        {
            foreach (var item in Constants.FileCategory.FileCategoryItems)
            {
                CategoryComboBox.Items.Add(item);
            }
        }

        private async void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            var fileName = String.Empty;

#if DEBUG
            fileName = //openFileDialog.FileName;
                       //@"E:\j\work\personal\projects\on3\test sheets\simple_test_3_urls.xlsx";
                        @"E:\j\work\personal\projects\on3\test sheets\softball&baseballurls.xlsx";
                        //@"E:\j\work\personal\projects\on3\test sheets\FieldHockeyD1URLs.xlsx";
#elif (!DEBUG)
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "Excel files (*.xlsx)|*.xlsx"
            };

            if (openFileDialog.ShowDialog() != true) return;

            fileName = openFileDialog.FileName;
#endif
            FileNameTextBox.Text = Path.GetFileName(fileName);

            var reader = new ExcelReader<RosterSheet>(fileName);
            var urls = reader.ReadSheet().ToList();

            if (!urls.Any())
            {
                MessageBox.Show("Error: Could not parse any URLs from the spreadsheet. Please try a different file.");
                return;
            }

            // Get URL type from UI dropdown and pass to crawling engine
            var category = CategoryComboBox.SelectionBoxItem.ToString();
            if (String.IsNullOrWhiteSpace(category) || category == Constants.FileCategory.DefaultValue)
            {
                MessageBox.Show("Select a URL Category to continue.");
                return;
            }

            // Start the crawling engine on a new thread
            await Task.Run(() => StartCrawlingEngineAsync(urls, category));
        }

        /// <summary>
        /// Starts the crawling engine.
        /// </summary>
        /// <param name="urls">The list of urls to crawl.</param>
        /// <param name="category">The category of these urls.</param>
        private async Task StartCrawlingEngineAsync(IEnumerable<RosterSheet> urls, string category)
        {
            var sheetUrls = urls.Select(t => t.Url);
            var manager = new EngineManager(new Crawler(sheetUrls), category, new QueueManager<CrawledPage>());
            await manager.StartAsync();
        }
    }
}
