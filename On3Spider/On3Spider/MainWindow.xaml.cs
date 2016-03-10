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
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "Excel files (*.xlsx)|*.xlsx"
            };

            if (openFileDialog.ShowDialog() != true) return;

            var reader = new ExcelReader(openFileDialog.FileName);
            var urls = reader.ReadUrls().ToList();

            if (!urls.Any())
            {
                return;
            }

            var crawler = new Crawler(urls);
        }
    }
}
