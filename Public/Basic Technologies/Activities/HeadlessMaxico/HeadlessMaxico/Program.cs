using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace HeadlessMaxico
{
    public static class WebDriverExtensions
    {
        public static IWebElement FindElement(this IWebDriver driver, By by, int timeoutInSeconds)
        {
            if (timeoutInSeconds > 0)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(drv => drv.FindElement(by));
            }
            return driver.FindElement(by);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            //M1();
            //DoSomething();
            var lines = ReadListUsingSeleniumAndFFoxHeadless();

            Console.ReadLine();
        }

        private static void M1()
        {
            string remoteUri = @"https://www.google.com/maps/d/u/0/viewer?mid=14q05ytl11J34y0EmQD9YerGPZldl8Uzh&ll=20.718274900301132%2C-106.29126836133636&z=7";

            var driver = new FirefoxDriver();
            driver.Navigate().GoToUrl(remoteUri);
            driver.FindElement(By.CssSelector(".HzV7m-pbTTYe-KoToPc-ornU0b-hFsbo")).Click();
            driver.FindElement(By.CssSelector(".pbTTYe-ibnC6b-d6wfac:nth-child(3) .suEOdc")).Click();
            var txt = driver.FindElement(By.CssSelector(".qqvbed-p83tee:nth-child(3) > .qqvbed-p83tee-lTBxed")).Text;
          
            // don't forget to kill the browser or else you'll have neverending chromedriver.exe processes
            driver.Quit();
        }

        private static void DoSomething()
        {
            string remoteUri = @"https://www.google.com/maps/d/u/0/viewer?mid=14q05ytl11J34y0EmQD9YerGPZldl8Uzh&ll=20.718274900301132%2C-106.29126836133636&z=7";
            var driver = new FirefoxDriver();
            driver.Navigate().GoToUrl(remoteUri);

            var elements = driver.FindElements(By.CssSelector(".pbTTYe-ibnC6b-d6wfac:nth-child(3) .suEOdc"));

            int muncipalityCount = elements.Count;



            string htmlSource = driver.PageSource;
            HtmlDocument doc = new HtmlDocument();
            
            
            
            
            doc.LoadHtml(htmlSource);


            var tableRows = new List<object>();
            try
            {
                //var elements = browser.FindElements(By.XPath($"//div[@class='suEOdc']"));
                //int index = 0;
                //foreach (var element in elements)
                //{
                //    if (++index <= 2) continue;

                //    element.Click();

                //    string htmlSource2 = browser.PageSource;
                //}

                var records = doc.DocumentNode.SelectNodes("//div[@class='suEOdc']");
                int index = 0;
                foreach (var div in records)
                {
                    if (++index <= 1) continue;

                    string pharmacyName = div.InnerText.Trim();
                    //var browserClone = browser.Clone();
                    driver.FindElement(By.XPath($"//div[@data-tooltip='{pharmacyName}']")).Click();
                    string htmlSource2 = driver.PageSource;

                    doc.LoadHtml(htmlSource2);
                    records = doc.DocumentNode.SelectNodes("//div[@class='qqvbed-p83tee-lTBxed']");

                    try
                    {
                        tableRows.Add(new
                        {
                            LocationName = div.InnerText.Trim(),
                            Address = "",
                            City = "",
                            Province = "",
                            Phone = "",
                            Announcement = "Next Available Appintment: ",
                            Website = $"",
                        });
                    }
                    catch (Exception ex)
                    {
                        ;
                    }
                }

            }
            catch (Exception ex)
            {
                ;
            }

        }

        public class RawItem
        {
            public string LocationName { get; set; }
            public string Region { get; set; }
            public int VaccineShipped { get; set; }
            public string Phone { get; set; }
            public string Website { get; set; }
            public string Address { get; set; }
            public decimal Latitude { get; set; }
            public decimal Longitude { get; set; }
            public string City { get; set; }
            public string PostalCode { get; set; }


            public static RawItem FromCsv(string csvLine)
            {
                string[] values = System.Text.RegularExpressions.Regex.Split(csvLine, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                RawItem item = new RawItem();
                item.LocationName = Convert.ToString(values[0]);
                item.Region = Convert.ToString(values[1]);
                item.VaccineShipped = Convert.ToInt32(values[2]);
                item.Phone = Convert.ToString(values[3]);
                item.Website = Convert.ToString(values[4]);
                item.Address = Convert.ToString(values[5]);
                item.Latitude = Convert.ToDecimal(values[values.Length - 4]);
                item.Longitude = Convert.ToDecimal(values[values.Length - 3]);
                item.City = Convert.ToString(values[values.Length - 2]);
                item.PostalCode = Convert.ToString(values[values.Length - 1]);

                return item;
            }
        }
        
        const string remoteUri = @"https://www.google.com/maps/d/u/0/viewer?mid=14q05ytl11J34y0EmQD9YerGPZldl8Uzh&ll=20.718274900301132%2C-106.29126836133636&z=7";
        const string country = @"Maxico";
        const string provider = @"maxico_gov";
        const string phone = @"";
        const string regUrl = @"";

        private static List<RawItem> ReadListUsingSeleniumAndFFoxHeadless()
        {
            //FirefoxOptions options = new FirefoxOptions();
            //options.AddArguments("--headless");
            //var driver = new FirefoxDriver(options);

            //System.setProperty("webdriver.gecko.driver", "C:/Program Files/Mozilla Firefox/geckodriver-v0.8.0-win32/geckodriver.exe");
            ////Now you can Initialize marionette driver to launch firefox
            //DesiredCapabilities capabilities = DesiredCapabilities.firefox();
            //capabilities.setCapability("marionette", true);
            //WebDriver driver = new FirefoxDriver(capabilities);
            string path = $"{country.ToLower()}_{provider.ToLower().Trim()}.csv";
            var existingRecords = ReadCsv(path);
            int startindex = existingRecords.Count() == 0 ? 1 : existingRecords.Count() + 1;


            var driver = new FirefoxDriver();
            driver.Navigate().GoToUrl(remoteUri);

            var elements = driver.FindElements(By.CssSelector(".pbTTYe-ibnC6b-d6wfac .suEOdc"));

            int muncipalityCount = elements.Count;
            List<RawItem> list = startindex == 1 ? new List<RawItem>() : existingRecords.ToList();
            for (int index = startindex; index < muncipalityCount; index++)
            {
                try
                {
                    list.Add(ReadMuncipalityDetailsItem(driver, index));
                    Console.WriteLine($"list contains {list.Count} elements of {muncipalityCount}");
                    // if (index == 2) break;

                    if (list.Count() % 10 == 0)
                    {
                        WriteCsv(list, path);
                    }
                }
                catch(Exception exx)
                {
                    ; //
                }
            }
            WriteCsv(list, path);
            Console.WriteLine($"Write operation completed");

            var newList = ReadCsv(path);

            driver.Quit();
            return list;
        }

        public static void WriteCsv<T>(IEnumerable<T> items, string path)
        {
            Type itemType = typeof(T);
            var props = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine(string.Join(", ", props.Select(p => p.Name)));

                foreach (var item in items)
                {
                    writer.WriteLine(string.Join(", ", props.Select(p => p.GetValue(item, null))));
                }
            }
        }

        public static IEnumerable<RawItem> ReadCsv(string path)
        {
            try
            {
                List<RawItem> list = File.ReadAllLines(path)
                                               .Skip(1)
                                               .Select(v => RawItem.FromCsv(v))
                                               .ToList();
                return list;
            }
            catch (Exception ex)
            {
                return new List<RawItem>();
            }
        }
        private static RawItem  ReadMuncipalityDetailsItem(FirefoxDriver driver, int index)
        {
             ////FirefoxOptions options = new FirefoxOptions();
            ////options.AddArguments("--headless");
            ////var driver = new FirefoxDriver(options);

            //var driver = new FirefoxDriver();
            driver.Navigate().GoToUrl(remoteUri);

            driver.FindElement(By.CssSelector(".HzV7m-pbTTYe-KoToPc-ornU0b-hFsbo")).Click();
            driver.FindElement(By.CssSelector($".pbTTYe-ibnC6b-d6wfac:nth-child({index}) .suEOdc")).Click();
            var txt = driver.FindElement(By.CssSelector(".qqvbed-p83tee:nth-child(3) > .qqvbed-p83tee-lTBxed")).Text;

            // don't forget to kill the browser or else you'll have neverending chromedriver.exe processes

            var item = new RawItem
            {
                LocationName = driver.FindElement(By.CssSelector(".qqvbed-p83tee:nth-child(1) > .qqvbed-p83tee-lTBxed")).Text,
                Region = driver.FindElement(By.CssSelector(".qqvbed-p83tee:nth-child(4) > .qqvbed-p83tee-lTBxed")).Text,
                VaccineShipped = 1, // int.Parse(driver.FindElement(By.CssSelector(".qqvbed-p83tee:nth-child(5) > .qqvbed-p83tee-lTBxed")).Text),
                Phone = "",
                Website = "https://mivacuna.salud.gob.mx/index.php"
            };

            driver.FindElement(By.CssSelector($".qqvbed-T3iPGc-LgbsSe-Bz112c")).Click();

            if (driver.WindowHandles.Count > 1)
            {
                var page2 = driver.SwitchTo().Window(driver.WindowHandles[1]);
                var url = page2.Url;

                // var sboxes = page2.FindElements(By.CssSelector(".tactile-searchbox-input"));

                string htmlSource = page2.PageSource;
                int idx = htmlSource.IndexOf("aria-label=\"Destination") + "aria-label=\"Destination".Length;
                int till = htmlSource.IndexOf("\"", idx);
                string address = htmlSource.Substring(idx, till - idx)?.Trim();
                address = address.Replace(",", ";");

                var tokens = url.Substring(url.LastIndexOf("@") + 1).Split(",");

                item.Address = address;
                item.Latitude = decimal.Parse(tokens[0]);
                item.Longitude = decimal.Parse(tokens[1]);
            }
 
            //driver.Quit();
            return item;
        }
    }
}
