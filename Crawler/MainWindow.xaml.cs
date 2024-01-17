using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System.Threading;
using System.IO;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.DevTools.V112.Accessibility;

namespace Crawler
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

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        static void ScrollDownToBottom(IWebDriver driver)
        {
            // Sử dụng JavaScript Executor để thực hiện cuộc cuộn
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            long lastHeight = (long)js.ExecuteScript("return document.body.scrollHeight;");

            while (true)
            {
                // Cuộn xuống cuối trang
                js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");

                // Chờ một khoảng thời gian ngắn để trang web tải dữ liệu mới
                System.Threading.Thread.Sleep(1000);

                // Lấy chiều cao mới của trang
                long newHeight = (long)js.ExecuteScript("return document.body.scrollHeight;");

                // Kiểm tra xem đã đến cuối trang hay chưa
                if (newHeight == lastHeight)
                {
                    break;
                }

                lastHeight = newHeight;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) //old source
        {
            // Khởi tạo crawler
            ChromeDriver driver = new ChromeDriver();
            driver.Url = "https://images.google.com/";
            driver.Navigate();
            //

            // Nhập từ khóa
            Thread.Sleep(500);
            IWebElement searchBar = driver.FindElement(By.XPath("/html/body/div[1]/div[3]/form/div[1]/div[1]/div[1]/div/div[2]/div/textarea"));
            searchBar.SendKeys($"{tbTarget.Text}\n");
            //
            
            //Scroll
            //ScrollDownToBottom(driver);

            Thread.Sleep(2000);

            IWebElement grand = driver.FindElement(By.Id("islrg"));
            IWebElement parent = grand.FindElement(By.ClassName("islrc"));
            List<IWebElement> childs = parent.FindElements(By.TagName("img")).ToList();

            string url = childs[0].GetAttribute("src");
            if (url != null)
            {
                if (childs[0].GetAttribute("data-deferred") == "1")
                {
                    url = url.Split(',')[1];
                    byte[] imageBytes = Convert.FromBase64String(url);

                    // Lưu dữ liệu binary thành tệp tin hình ảnh
                    File.WriteAllBytes("exia.jpg", imageBytes);
                }
            }
        }

        private void thumbnail()
        {
            // Khởi tạo crawler
            ChromeDriver driver = new ChromeDriver();
            driver.Url = "https://images.google.com/";
            driver.Navigate();
            //

            // Nhập từ khóa
            Thread.Sleep(500);
            string target = tbTarget.Text;
            IWebElement searchBar = driver.FindElement(By.XPath("/html/body/div[1]/div[3]/form/div[1]/div[1]/div[1]/div/div[2]/div/textarea"));
            searchBar.SendKeys($"{target}\n");
            //DicCheck
            string path = $".\\ImgStorage\\{target}-thumbnail";
            if (Directory.Exists(path))
            {
                MessageBox.Show("Đối tượng đã tồn tại.");
                return;
            }
            else
            {
                Directory.CreateDirectory(path);
            }
            //Scroll
            ScrollDownToBottom(driver);

            Thread.Sleep(2000);
            //Crawl
            IWebElement grand = driver.FindElement(By.Id("islrg"));
            IWebElement parent = grand.FindElement(By.ClassName("islrc"));
            List<IWebElement> childs = parent.FindElements(By.TagName("img")).ToList();

            int quantity = Math.Min(childs.Count, Convert.ToInt32(tbCount.Text));
            int count = 0;
            for (int i = 0; i < childs.Count; i++)
            {
                if (count >= quantity) break;
                string url = childs[i].GetAttribute("src");
                if (url != null)
                {
                    if (childs[i].GetAttribute("jsname") == "Q4LuWd")
                    {
                        url = url.Split(',')[1];
                        byte[] imageBytes = Convert.FromBase64String(url);

                        // Lưu dữ liệu binary thành tệp tin hình ảnh
                        File.WriteAllBytes($"ImgStorage/{target}-thumbnail/{target}{count + 1}.jpg", imageBytes);
                        count++;
                    }
                }
            }
            MessageBox.Show("Done crawling.", "Thông báo");
        }

        private void fullRes()
        {
            // Khởi tạo crawler
            ChromeDriver driver = new ChromeDriver();
            driver.Url = "https://images.google.com/";
            driver.Navigate();
            //

            // Nhập từ khóa
            Thread.Sleep(500);
            string target = tbTarget.Text;
            IWebElement searchBar = driver.FindElement(By.XPath("/html/body/div[1]/div[3]/form/div[1]/div[1]/div[1]/div/div[2]/div/textarea"));
            searchBar.SendKeys($"{target}\n");
            //DicCheck
            string path = $".\\ImgStorage\\{target}-fullres";
            if (Directory.Exists(path))
            {
                MessageBox.Show("Đối tượng đã tồn tại.");
                return;
            }
            else
            {
                Directory.CreateDirectory(path);
            }
            //Scroll
            ScrollDownToBottom(driver);

            Thread.Sleep(2000);

            IWebElement grand = driver.FindElement(By.Id("islrg"));
            IWebElement parent = grand.FindElement(By.ClassName("islrc"));
            List<IWebElement> childs = parent.FindElements(By.TagName("img")).ToList();
            //Crawl
            int quantity = Math.Min(childs.Count, Convert.ToInt32(tbCount.Text));
            int count = 0;
            for (int i = 0; i < childs.Count; i++)
            {
                if (count >= quantity) break;
                string url = childs[i].GetAttribute("src");
                if (url != null)
                {
                    if (childs[i].GetAttribute("jsname") == "Q4LuWd")
                    {
                        childs[i].Click();
                        IWebElement fullImg = driver.FindElement(By.Id("Sva75c"));
                        fullImg = driver.FindElement(By.XPath("/html/body/div[2]/c-wiz/div[3]/div[2]/div[3]/div[2]/div[2]/div[2]/div[2]/c-wiz/div/div/div/div/div[3]/div[1]/a/img"));
                                                             ///html/body/div[2]/c-wiz/div[3]/div[2]/div[3]
                        count++;
                        Thread.Sleep(500);
                    }
                }
            }
            MessageBox.Show("Done crawling.", "Thông báo");
        }

        private void autoCrawl(object sender, RoutedEventArgs e)
        {
            if (chbFull.IsChecked == true)
            {
                fullRes();
            }
            else
            {
                thumbnail();
            }
        }

        private void QuantityText(object sender, TextChangedEventArgs e)
        {
            string quant = tbCount.Text;
            for (int i = 0; i < quant.Length; i++)
            {
                if (!('0' <= quant[i] && quant[i] <= '9'))
                {
                    quant = quant.Substring(0, i) + quant.Substring(i + 1);
                    i--;
                }
            }
            tbCount.Text = quant;
        }
    }
}